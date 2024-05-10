# This script prunes fonts to only include the characters needed by the translation + the set of base ascii characters
# This reduces the nro size from 16MB to 8MB since the chinese fonts take a lot of space

import json
import os
from fontTools.ttLib import TTFont
from fontTools.subset import Subsetter

FONT_FOLDER = "fonts"
FONT_ROMFS = "romfs/fonts"
STRINGS_ROMFS = "romfs/strings"

# Make a list of all the characters we need, this is done by loading the string jsons
characters = set()
for root, dirs, files in os.walk(STRINGS_ROMFS):
	for file in files:
		if file.endswith(".json"):
			with open(os.path.join(root, file), "r", encoding="utf-8") as f:
				data = json.load(f)
				
				def recursive_json_visit(element):
					if isinstance(element, str):
						for c in element:
							characters.add(c)
					elif isinstance(element, dict):
						for key in element:
							recursive_json_visit(element[key])
					elif isinstance(element, list):
						for item in element:
							recursive_json_visit(item)
				
				recursive_json_visit(data)

print("Characters needed:", len(characters))

subs = Subsetter()
subs.populate(text="".join(characters))
# add ascii
subs.populate(text="".join(chr(i) for i in range(32, 127)))

# Then go through the fonts
for root, dirs, files in os.walk(FONT_FOLDER):
	for file in files:
		if file.endswith(".ttf"):
			print("Processing " + file + " ...")
			# if the font is less than 2MB then we can keep it as is
			file_size = os.path.getsize(os.path.join(root, file))
			if file_size < 2 * 1024 * 1024:
				print("Font is less than 2MB, skipping")
				continue
			# Otherwise we need to prune it
			font = TTFont(os.path.join(root, file))
			subs.subset(font)
			font.save(os.path.join(FONT_ROMFS, file))
			font.close()

			new_size = os.path.getsize(os.path.join(FONT_ROMFS, file))
			print(f"Pruned from {file_size} to {new_size}", )
