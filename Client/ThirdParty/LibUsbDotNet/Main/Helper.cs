// Copyright © 2006-2010 Travis Robinson. All rights reserved.
// Copyright © 2011-2023 LibUsbDotNet contributors. All rights reserved.
// 
// website: http://github.com/libusbdotnet/libusbdotnet
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
// for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA. or 
// visit www.gnu.org.
// 
//

using System.Text;

namespace LibUsbDotNet.Main;

/// <summary>
/// General utilities class used by LudnLite and exposed publicly for your convience.
/// </summary>
public static class Helper
{
    /// <summary>
    /// Builds a delimited string of names and values.
    /// </summary>
    /// <param name="sep0">Inserted and the begining of the entity.</param>
    /// <param name="names">The list of names for the object values.</param>
    /// <param name="sep1">Inserted between the name and value.</param>
    /// <param name="values">The values for the names.</param>
    /// <param name="sep2">Inserted and the end of the entity.</param>
    /// <returns>The formatted string.</returns>
    public static string ToString(string sep0, string[] names, string sep1, object[] values, string sep2)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < names.Length; i++)
        {
            sb.Append(sep0 + names[i] + sep1 + values[i] + sep2);
        }

        return sb.ToString();
    }
}