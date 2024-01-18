using ImGuiNET;
using LibUsbDotNet;
using SysDVR.Client.Core;
using SysDVR.Client.Platform;
using SysDVR.Client.Targets.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace SysDVR.Client.GUI
{
	internal class OptionsView : View
	{
		// Support classes
		record struct Opt<T>(string Name, T Value);

		class ComboEnum<T>
		{
			public int CurrentItem;
			public readonly Opt<T>[] values;

			public readonly Memory<byte> ValuesUtf8Encoded;
			public readonly Memory<byte> Label;
			readonly Memory<byte> IdLabel;

			public ComboEnum(string label, Opt<T>[] values, T current)
			{
				IdLabel = Encoding.UTF8.GetBytes("##select_" + label);
				Label = Encoding.UTF8.GetBytes(label);

				this.values = values;
				
				CurrentItem = Array.FindIndex(values, x => x.Value switch { 
					null when current is null => true,
					null => false,
					_ => x.Value.Equals(current)
				});

				// Precompute labels as utf8 for ImGui
				var lengths = values.Select(x => Encoding.UTF8.GetByteCount(x.Name) + 1).ToArray();
				ValuesUtf8Encoded = new byte[lengths.Sum()];

				Span<byte> buffer = ValuesUtf8Encoded.Span;
				foreach (var (opt, len) in values.Zip(lengths))
				{
					Encoding.UTF8.GetBytes(opt.Name, buffer);
					buffer = buffer.Slice(len);
				}
			}

			public bool Draw(ref T outval)
			{
				ImGuiNative.igText(in MemoryMarshal.GetReference(Label.Span));
				ImGui.SameLine();
				byte ret = ImGuiNative.igCombo_Str(
					in MemoryMarshal.GetReference(IdLabel.Span),
					ref CurrentItem,
					in MemoryMarshal.GetReference(ValuesUtf8Encoded.Span),
					-1);

				if (ret != 0)
				{
					outval = values[CurrentItem].Value;
					return true;
				}

				return false;
			}
		}

		class PathInputPopup
		{
			public readonly Gui.Popup Popup = new("Select path");
			Gui.CenterGroup PathPopButtons = new();
			string PathInputMessage;
			readonly byte[] PathInputBuffer = new byte[1024];
			Action<string> PathPopupApply = null!;

			public void Configure(string message, string currentValue, Action<string> setvalue)
			{
				PathInputMessage = message;
				PathInputBuffer.AsSpan().Fill(0);
				Encoding.UTF8.GetBytes(currentValue, PathInputBuffer);
				PathPopupApply = setvalue;
			}

			public void Draw()
			{
				if (!Popup.Begin())
					return;

				ImGui.TextWrapped(PathInputMessage);
				ImGui.SetNextItemWidth(ImGui.GetWindowWidth());
				ImGui.InputText("##pathpopinput", PathInputBuffer, (uint)PathInputBuffer.Length);

				PathPopButtons.StartHere();

				if (ImGui.Button("Cancel"))
					Popup.RequestClose();
				ImGui.SameLine();
				if (ImGui.Button("Save"))
				{
					var stopAt = Array.IndexOf(PathInputBuffer, (byte)0);
					string path = "";

					if (stopAt > 0)
						path = Encoding.UTF8.GetString(PathInputBuffer, 0, stopAt).Trim();

					if (!Directory.Exists(path))
					{
						if (!PathInputMessage.Contains("does not exist"))
							PathInputMessage += "\nThe selected path does not exist, try again.";
					}
					else
					{
						PathPopupApply(path);
						Popup.RequestClose();
					}
				}

				PathPopButtons.EndHere();

				ImGui.EndPopup();
			}
		}

		record AvailableLanguage(string Name, string Author, string Locale);

		// Instance state 
		readonly StringTable.SettingsTable Strings = Program.Strings.Settings;

		// These can not refer to a non-static member so they use the fully quelified strings object
		readonly ComboEnum<SDLScaleMode> ScaleModes = new(Program.Strings.Settings.ScaleMode, new Opt<SDLScaleMode>[]
			{
				new(Program.Strings.Settings.ScaleMode_Linear, SDLScaleMode.Linear),
				new(Program.Strings.Settings.ScaleMode_Narest, SDLScaleMode.Nearest),
				new(Program.Strings.Settings.ScaleMode_Best, SDLScaleMode.Best)
			},
			Program.Options.RendererScale
		);

		readonly ComboEnum<SDLAudioMode> AudioModes = new(Program.Strings.Settings.AudioMode, new Opt<SDLAudioMode>[]
			{
				new(Program.Strings.Settings.AudioMode_Auto, SDLAudioMode.Auto),
				new(Program.Strings.Settings.AudioMode_Sync, SDLAudioMode.Default),
				new(Program.Strings.Settings.AudioMode_Compatible, SDLAudioMode.Compatible)
			},
			Program.Options.AudioPlayerMode
		);

		readonly ComboEnum<StreamKind> StreamChannel = new(Program.Strings.Settings.DefaultStreaming, new Opt<StreamKind>[]
			{
				new(Program.Strings.Settings.DefaultStreaming_Both, StreamKind.Both),
				new(Program.Strings.Settings.DefaultStreaming_Video, StreamKind.Video),
				new(Program.Strings.Settings.DefaultStreaming_Audio, StreamKind.Audio)
			},
			Program.Options.Streaming.Kind
		);

		readonly ComboEnum<string> GuiLanguage = new("Display language",
			Resources.GetAvailableTranslations().Select(x =>
				new Opt<string?>($"{x.TranslationName} by {x.TranslationAuthor}", x.SystemLocale.First())
			)
			.Append(new("Auto select", null))
			.ToArray(), 
		Program.Options.PreferredLanguage);

		readonly PathInputPopup PathInput = new();
		readonly Gui.Popup ErrorPopup = new(Program.Strings.General.PopupErrorTitle);
		string SettingsErrorMessage = "";
		Gui.CenterGroup SaveCenter = new();

		readonly Gui.Popup PickDecoderPopup = new(Program.Strings.Settings.DecoderPopupTitle);
		string DecoderButtonText;
		List<LibavUtils.Codec> PickDecoderList = new();

		void UpdateDecoderButtonText()
		{
			if (Program.Options.DecoderName is not null)
				DecoderButtonText = string.Format(Strings.DecoderResetButton,  Program.Options.DecoderName);
			else
				DecoderButtonText = Strings.DecoderChangeButton;
		}

		public OptionsView()
		{
			Popups.Add(PathInput.Popup);
			Popups.Add(ErrorPopup);
			Popups.Add(PickDecoderPopup);
			UpdateDecoderButtonText();
		}

		public void OpenSelectPath(string message, string currentValue, Action<string> setvalue)
		{
			PathInput.Configure(message, currentValue, setvalue);
			Popups.Open(PathInput.Popup);
		}

		void SaveOptions()
		{
			try
			{
				SystemUtil.StoreSettingsString(Program.Options.SerializeToJson());
			}
			catch (Exception e)
			{
				SettingsErrorMessage = $"{Strings.SaveFailedError}:\r\n{e}";
				Popups.Open(ErrorPopup);
			}
		}

		string changeRecordingButton = Program.Strings.Settings.ChangePathButton + "##clip";
		string changePicButton = Program.Strings.Settings.ChangePathButton + "##pic";
		public override void Draw()
		{
			if (!Gui.BeginWindow("Settings"))
				return;

			ImGui.TextWrapped(Strings.Heading);

			SaveCenter.StartHere();
			if (ImGui.Button(GeneralStrings.BackButton))
				Program.Instance.PopView();

			ImGui.SameLine();
			if (ImGui.Button(Strings.SaveButton))
				SaveOptions();

			ImGui.SameLine();
			if (ImGui.Button(Strings.ResetButton))
			{
				Program.Options = new();
				SaveOptions();
			}

			SaveCenter.EndHere();

			Gui.CenterText(Strings.RestartWarnLabel);
			ImGui.NewLine();

			if (ImGui.CollapsingHeader(Strings.Tab_General, ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Indent();

				GuiLanguage.Draw(ref Program.Options.PreferredLanguage);
				ImGui.TextWrapped("Translations are provided by the community, if you spot an error or want to contribute your native language reach out to us on Github.");

				ImGui.NewLine();

				ImGui.Checkbox(Strings.HideSerials, ref Program.Options.HideSerials);
				ImGui.Checkbox(Strings.Hotkeys, ref Program.Options.PlayerHotkeys);
				ScaleModes.Draw(ref Program.Options.RendererScale);
				AudioModes.Draw(ref Program.Options.AudioPlayerMode);

				// Recording save path, TODO: implement file picker but doing it cross platform seems like a major headache
				ImGui.TextWrapped(Strings.RecordingsOutputPath);
				ImGui.Indent();
				ImGui.TextWrapped(Program.Options.RecordingsPath);
				ImGui.SameLine();
				if (ImGui.Button(changeRecordingButton))
					OpenSelectPath(Strings.RecordingsOutputDialogTitle, Program.Options.RecordingsPath, x => Program.Options.RecordingsPath = x);
				ImGui.Unindent();

				ImGui.TextWrapped(Strings.ScreenshotOutputPath);
				ImGui.Indent();
				ImGui.TextWrapped(Program.Options.ScreenshotsPath);
				ImGui.SameLine();
				if (ImGui.Button(changePicButton))
					OpenSelectPath(Strings.ScreenshotOutputDialogTitle, Program.Options.ScreenshotsPath, x => Program.Options.ScreenshotsPath = x);
				ImGui.Unindent();

				if (Program.IsWindows)
					ImGui.Checkbox(Strings.ScreenshotToClipboard, ref Program.Options.Windows_ScreenToClip);

				StreamChannel.Draw(ref Program.Options.Streaming.Kind);

				ImGui.Unindent();
				ImGui.NewLine();
			}

			if (ImGui.CollapsingHeader(Strings.Tab_Performance, ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Indent();

				ImGui.TextWrapped(Strings.PerformanceRenderingLabel);
				ImGui.Checkbox(Strings.UncapStreaming, ref Program.Options.UncapStreaming);
				ImGui.Checkbox(Strings.UncapGUI, ref Program.Options.UncapGUI);

				ImGui.NewLine();
				ImGui.TextWrapped(Strings.PerformanceStreamingLabel);

				ImGui.Text(Strings.AudioBatching); ImGui.SameLine();
				ImGui.SliderInt("##SliderAudioB", ref Program.Options.Streaming.AudioBatching, 0, StreamInfo.MaxAudioBatching);

				ImGui.Checkbox(Strings.CachePackets, ref Program.Options.Streaming.UseNALReplay);
				ImGui.Checkbox(Strings.CachePacketsKeyframes, ref Program.Options.Streaming.UseNALReplayOnlyOnKeyframes);

				ImGui.Unindent();
				ImGui.NewLine();
			}

			if (ImGui.CollapsingHeader(Strings.Tab_Advanced, ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Indent();

				ImGui.Checkbox(Strings.ForceSDLSoftwareRenderer, ref Program.Options.ForceSoftwareRenderer);
				ImGui.Checkbox(Strings.PrintRealtimeLogs, ref Program.Options.Debug.Stats);
				ImGui.Checkbox(Strings.VerboseDebugging, ref Program.Options.Debug.Log);
				ImGui.Checkbox(Strings.DisableSynchronization, ref Program.Options.Debug.NoSync);
				ImGui.NewLine();

				ImGui.Checkbox(Strings.AnalyzeKeyframes, ref Program.Options.Debug.Keyframe);
				ImGui.Checkbox(Strings.AnalyzeNALs, ref Program.Options.Debug.Nal);

				ImGui.Text(Strings.GuiScale); ImGui.SameLine();
				ImGui.SliderFloat("##sliderGuiScale", ref Program.Options.GuiFontScale, 0.1f, 4);

				ImGui.NewLine();
				if (ImGui.Button(DecoderButtonText))
				{
					if (Program.Options.DecoderName is not null)
					{
						Program.Options.DecoderName = null;
						UpdateDecoderButtonText();
					}
					else
					{
						ShowDecoderPickPopup();
					}
				}

				// TODO:
				// Usb log level
				// other debug options

				ImGui.Unindent();
				ImGui.NewLine();
			}

			PathInput.Draw();
			DrawErrorPopup();
			DrawDecoderPickerPopup();

			Gui.EndWindow();
		}

		void ShowDecoderPickPopup() 
		{
			Popups.Open(PickDecoderPopup);
			PickDecoderList.Clear();
			PickDecoderList.AddRange(LibavUtils.GetH264Decoders());
		}

		void DrawDecoderPickerPopup()
		{
			if (PickDecoderPopup.Begin(ImGui.GetWindowSize()))
			{
				Gui.CenterText(Strings.DecderPopupHeading);
				ImGui.TextWrapped(Strings.DecderPopupContent);

				ImGui.NewLine();

				var sz = ImGui.GetContentRegionMax() - new Vector2(10, ImGui.GetCursorPosY() + 100);

				if (sz.Y < 100)
					sz.Y = 100;

				ImGui.BeginChildFrame(ImGui.GetID("##CodecList"), sz, ImGuiWindowFlags.NavFlattened);
				foreach (var c in PickDecoderList)
				{
					if (ImGui.Button($"{c.Name}: {c.Description}", new Vector2(sz.X - 10, 0)))
					{
						Program.Options.DecoderName = c.Name;
						UpdateDecoderButtonText();
						PickDecoderPopup.RequestClose();
					}
				}
				Gui.MakeWindowScrollable();
				ImGui.EndChildFrame();

				ImGui.NewLine();

				if (Gui.CenterButton(GeneralStrings.CancelButton))
					PickDecoderPopup.RequestClose();
			}
		}

		void DrawErrorPopup()
		{
			if (ErrorPopup.Begin())
			{
				ImGui.TextWrapped(SettingsErrorMessage);
				ImGui.NewLine();

				if (Gui.CenterButton(GeneralStrings.PopupCloseButton))
				{
					SettingsErrorMessage = "";
					ErrorPopup.RequestClose();
				}

				ImGui.EndPopup();
			}
		}
	}
}
