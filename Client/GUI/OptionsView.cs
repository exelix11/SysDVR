﻿using ImGuiNET;
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
				CurrentItem = Array.FindIndex(values, x => x.Value.Equals(current));

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

		// Instance state 

		readonly ComboEnum<SDLScaleMode> ScaleModes = new("Scale mode", new Opt<SDLScaleMode>[]
			{
				new("Linear (default)", SDLScaleMode.Linear),
				new("Nearest (low overhead)", SDLScaleMode.Nearest),
				new("Best (high quality, up to the system)", SDLScaleMode.Best)
			},
			Program.Options.RendererScale
		);

		readonly ComboEnum<SDLAudioMode> AudioModes = new("Audio player mode", new Opt<SDLAudioMode>[]
			{
				new("Automatic (default)", SDLAudioMode.Auto),
				new("Synchronized", SDLAudioMode.Default),
				new("Compatible (try it if you have audio issues)", SDLAudioMode.Compatible)
			},
			Program.Options.AudioPlayerMode
		);

		readonly ComboEnum<StreamKind> StreamChannel = new("Default streaming channel", new Opt<StreamKind>[]
			{
				new("Both (default)", StreamKind.Both),
				new("Video only", StreamKind.Video),
				new("Audio only", StreamKind.Audio)
			},
			Program.Options.Streaming.Kind
		);

		readonly PathInputPopup PathInput = new();
		readonly Gui.Popup ErrorPopup = new("Settings error");
		string SettingsErrorMessage = "";
		Gui.CenterGroup SaveCenter = new();

		readonly Gui.Popup PickDecoderPopup = new("Select video decoder");
		string DecoderButtonText;
		List<LibavUtils.Codec> PickDecoderList = new();

		void UpdateDecoderButtonText()
		{
			if (Program.Options.DecoderName is not null)
				DecoderButtonText = $"Disable hardware decoder ({Program.Options.DecoderName})";
			else
				DecoderButtonText = "Configure hardware-accelerated decoder";
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
				SettingsErrorMessage = "Failed to save settings:\r\n" + e.ToString();
				Popups.Open(ErrorPopup);
			}
		}

		public override void Draw()
		{
			if (!Gui.BeginWindow("Settings"))
				return;

			ImGui.TextWrapped("These settings are automatically applied for the current session, you can however save them so they become persistent");

			SaveCenter.StartHere();
			if (ImGui.Button("Go back"))
				Program.Instance.PopView();

			ImGui.SameLine();
			if (ImGui.Button("Save changes"))
				SaveOptions();

			ImGui.SameLine();
			if (ImGui.Button("Reset defaults"))
			{
				Program.Options = new();
				SaveOptions();
			}

			SaveCenter.EndHere();

			Gui.CenterText("Some changes may require to restart SysDVR");
			ImGui.NewLine();

			if (ImGui.CollapsingHeader("General", ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Indent();

				ImGui.Checkbox("Hide console serials from GUI", ref Program.Options.HideSerials);
				ImGui.Checkbox("Enable hotkeys in the player view", ref Program.Options.PlayerHotkeys);
				ScaleModes.Draw(ref Program.Options.RendererScale);
				AudioModes.Draw(ref Program.Options.AudioPlayerMode);

				// Recording save path, TODO: implement file picker but doing it cross platform seems like a major headache
				ImGui.TextWrapped("Video recordings output path:");
				ImGui.Indent();
				ImGui.TextWrapped(Program.Options.RecordingsPath);
				ImGui.SameLine();
				if (ImGui.Button("Change##video"))
					OpenSelectPath("Select the video recording output path", Program.Options.RecordingsPath, x => Program.Options.RecordingsPath = x);
				ImGui.Unindent();

				ImGui.TextWrapped("Screenshots output path:");
				ImGui.Indent();
				ImGui.TextWrapped(Program.Options.ScreenshotsPath);
				ImGui.SameLine();
				if (ImGui.Button("Change##screen"))
					OpenSelectPath("Select the screenshots output path", Program.Options.ScreenshotsPath, x => Program.Options.ScreenshotsPath = x);
				ImGui.Unindent();

				if (Program.IsWindows)
					ImGui.Checkbox("Copy screenshots to the clipboard instead of saving as files (Press SHIFT to override during capture)", ref Program.Options.Windows_ScreenToClip);

				StreamChannel.Draw(ref Program.Options.Streaming.Kind);

				ImGui.Unindent();
				ImGui.NewLine();
			}

			if (ImGui.CollapsingHeader("Performance", ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Indent();

				ImGui.TextWrapped("These options affect the rendering pipeline of the client, when enabling 'uncapped' modes SysDVR-client will sync to the vsync event of your device, this should remove any latency due to the rendering pipeline but mey use more power on mobile devices.");
				ImGui.Checkbox("Uncap streaming framerate", ref Program.Options.UncapStreaming);
				ImGui.Checkbox("Uncap GUI framerate", ref Program.Options.UncapGUI);

				ImGui.NewLine();
				ImGui.TextWrapped("These options affect the straming quality of SysDVR, the defaults are usually fine");

				ImGui.Text("Audio batching"); ImGui.SameLine();
				ImGui.SliderInt("##SliderAudioB", ref Program.Options.Streaming.AudioBatching, 0, StreamInfo.MaxAudioBatching);

				ImGui.Checkbox("Cache video packets (NAL) locally and replay them when needed", ref Program.Options.Streaming.UseNALReplay);
				ImGui.Checkbox("Apply packet cache only to keyframes (H264 IDR frames)", ref Program.Options.Streaming.UseNALReplayOnlyOnKeyframes);

				ImGui.Unindent();
				ImGui.NewLine();
			}

			if (ImGui.CollapsingHeader("Advanced", ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Indent();

				ImGui.Checkbox("Force SDL software rendering", ref Program.Options.ForceSoftwareRenderer);
				ImGui.Checkbox("Print real-time streaming information", ref Program.Options.Debug.Stats);
				ImGui.Checkbox("Enable verbose logging", ref Program.Options.Debug.Log);
				ImGui.Checkbox("Disable Audio/Video synchronization", ref Program.Options.Debug.NoSync);
				ImGui.NewLine();

				ImGui.Checkbox("Analyze keyframe NALs during the stream", ref Program.Options.Debug.Keyframe);
				ImGui.Checkbox("Analyze every NAL during the stream", ref Program.Options.Debug.Nal);

				ImGui.Text("GUI scale"); ImGui.SameLine();
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
				Gui.CenterText("Select a decoder to use");
				ImGui.TextWrapped("Note that not all decoders may be compatible with your system, revert this option in case of issues.\n" +
					"You can get more decoders by obtaining a custom build of ffmpeg libraries (libavcodec) and replacing the one included in SysDVR-Client.\n\n" +
					"This feature is intended for mini-PCs like Raspberry pi where software decoding might not be enough. On desktop PCs and smartphones this option should not be used.");

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

				if (Gui.CenterButton("Cancel"))
					PickDecoderPopup.RequestClose();
			}
		}

		void DrawErrorPopup()
		{
			if (ErrorPopup.Begin())
			{
				ImGui.TextWrapped(SettingsErrorMessage);
				ImGui.NewLine();

				if (Gui.CenterButton("Close"))
				{
					SettingsErrorMessage = "";
					ErrorPopup.RequestClose();
				}

				ImGui.EndPopup();
			}
		}
	}
}
