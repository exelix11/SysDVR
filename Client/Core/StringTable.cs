using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SysDVR.Client.Core
{
	internal class StringTableMetadata
	{
		[JsonIgnore]
		public string FileName;

		// This is the font that will be loaded to render UI-text, you can use this field to provide a custom font for rendering unsupported characters
		public string FontName = "OpenSans.ttf";

		// These are just metadata fields shown in the settings page and do not affect the behavior of translation loading
		public string TranslationName = "English";
		public string TranslationAuthor = "SysDVR";

		// This is a list of locales that will be used to to load translations
		// The values come from the CultureInfo.Name property of c# https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.name?view=net-8.0#system-globalization-cultureinfo-name
		// See examples at http://www.codedigest.com/CodeDigest/207-Get-All-Language-Country-Code-List-for-all-Culture-in-C---ASP-Net.aspx
		public string[] SystemLocale = ["en-us"];

		public enum GlyphRange 
		{
			Auto,
			ChineseFull,
			ChineseSimplifiedCommon,
			Cyrillic,
			Default,
			Greek,
			Japanese,
			Korean,
			Thai,
			Vietnamese
		}

		// This is the glyph range as defined i	n ImGUi, see https://github.com/ocornut/imgui/blob/master/docs/FONTS.md#fonts-loading-instructions
		public GlyphRange ImGuiGlyphRange = GlyphRange.Auto;
	}

	// This class is used to translate the client UI in other languages
	// This file only contains the default language which is used as a fallback when a translation is missing
	// You should modify the reference json file in the Resources/Translations folder
	// Please read the translation guide at <TBD>
	internal class StringTable : StringTableMetadata
	{
		// The following are "legacy player" strings, they are only printed to the console when using the legacy player
		public string LegacyPlayer_Starting = "Starting in legacy mode...";
		public string LegacyPlayer_Started = 
			"Starting to stream, keyboard shortcuts list:\n" +
			"\t- F11 : toggle full screen\n" +
			"\t- esc :quit\n" +
			"\t- return: Print debug information";
		public string LegacyPlayer_AudioOnly = "No video output requested, press return to quit";

		// The following are general strings used in multiple places
		public string MainGUI_PopupCloseButton = "Close";

		// The following are strings used by the main GUI page
		public string MainGUI_InitializationErrorTitle = "Initialization error";
		public string MainGUI_NetworkButton = "Network mode";
		public string MainGUI_USBButton = "USB mode";
		public string MainGUI_ChannelLabel = "Select the streaming mode";
		public string MainGUI_ChannelVideo = "Video only";
		public string MainGUI_ChannelAudio = "Audio only";
		public string MainGUI_ChannelBoth = "Stream Both";
		
		public string MainGUI_SettingsButton = "Settings";
		public string MainGUI_GuideButton = "Guide";
		public string MainGUI_GithubButton = "Github page";

		// The following are only shown on android
		public string MainGUI_FileAccess = "Warning: File access permission was not granted, saving recordings may fail.";
		public string MainGUI_FileAccessRequestButton = "Request permission";

		// The following are only shown on windows
		public string MainGUI_DriverInstallButton = "USB driver";

		// Used internally to produce the reference english translation file
		internal string Serialize() 
		{
			return JsonSerializer.Serialize(this, StringTableSerializer.Default.SysDVRStringTable);
		}
	}

	// When using AOT we must use source generation for json serialization since reflection is not available
	[JsonSourceGenerationOptions(
		WriteIndented = true,
		PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		IncludeFields = true,
		UseStringEnumConverter = true,
		IgnoreReadOnlyProperties = true)]
	[JsonSerializable(typeof(StringTableMetadata), TypeInfoPropertyName = "SysDVRStringTableMetadata")]
	[JsonSerializable(typeof(StringTable), TypeInfoPropertyName = "SysDVRStringTable")]
	internal partial class StringTableSerializer : JsonSerializerContext
	{
	}
}
