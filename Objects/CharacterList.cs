using System;
using System.Collections.Generic;
using System.Text;

namespace KarelazisBot.Objects
{
    public class CharacterList
    {
        public CharacterList(Objects.Client c)
        {
            this.Client = c;
        }

        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets or sets the amount of premium days this account has.
        /// </summary>
        public ushort PremiumDays
        {
            get { return this.Client.Memory.ReadUInt16(this.Client.Addresses.CharacterList.PremiumDays); }
            set { this.Client.Memory.WriteUInt16(this.Client.Addresses.CharacterList.PremiumDays, value); }
        }
        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        public byte SelectedIndex
        {
            get { return this.Client.Memory.ReadByte(this.Client.Addresses.CharacterList.SelectedIndex); }
            set { this.Client.Memory.WriteByte(this.Client.Addresses.CharacterList.SelectedIndex, value); }
        }
        /// <summary>
        /// Gets the amount of characters currently loaded into the character list.
        /// </summary>
        public byte Count
        {
            get { return this.Client.Memory.ReadByte(this.Client.Addresses.CharacterList.Amount); }
        }

        /// <summary>
        /// Gets the characters of this account.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Character> GetCharacters()
        {
            int characterListStart = this.Client.Memory.ReadInt32(this.Client.Addresses.CharacterList.Pointer),
                count = this.Client.Memory.ReadInt32(this.Client.Addresses.CharacterList.Amount);
            byte index = 0;
            for (int i = characterListStart;
                i <= characterListStart + count * this.Client.Addresses.CharacterList.Step;
                i += this.Client.Addresses.CharacterList.Step)
            {
                yield return new CharacterList.Character(index,
                    this.Client.Memory.ReadString(i + this.Client.Addresses.CharacterList.Distances.CharacterName),
                    this.Client.Memory.ReadString(i + this.Client.Addresses.CharacterList.Distances.ServerName),
                    this.Client.Memory.ReadString(i + this.Client.Addresses.CharacterList.Distances.ServerIP),
                    this.Client.Memory.ReadUInt16(i + this.Client.Addresses.CharacterList.Distances.ServerPort));
                index++;
            }
        }
        /// <summary>
        /// Gets the currently logged in character. Returns null if not logged in.
        /// </summary>
        /// <returns></returns>
        public Character GetCurrentCharacter()
        {
            List<Character> chars = new List<Character>(this.GetCharacters());
            if (chars.Count > 0) return chars[this.SelectedIndex];
            else return null;
        }
        public void SetCharacter(Character character)
        {
            int start = this.Client.Memory.ReadInt32(this.Client.Addresses.CharacterList.Pointer),
                count = this.Client.Memory.ReadInt32(this.Client.Addresses.CharacterList.Amount);

            if (count <= character.Index) return;

            start += this.Client.Addresses.CharacterList.Step * character.Index;
            this.Client.Memory.WriteString(start + this.Client.Addresses.CharacterList.Distances.CharacterName,
                character.Name);
            this.Client.Memory.WriteString(start + this.Client.Addresses.CharacterList.Distances.ServerName,
                character.ServerName);
            this.Client.Memory.WriteString(start + this.Client.Addresses.CharacterList.Distances.ServerIP,
                character.ServerIP);
            this.Client.Memory.WriteUInt16(start + this.Client.Addresses.CharacterList.Distances.ServerPort,
                character.ServerPort);
        }
        public void SetCharacters(IEnumerable<Character> characters)
        {
            foreach (Character c in characters) this.SetCharacter(c);
        }

        public class Character
        {
            public Character() { }
            public Character(byte index, string name, string serverName, string serverIP, ushort serverPort)
            {
                this.Index = index;
                this.Name = name;
                this.ServerName = serverName;
                this.ServerIP = serverIP;
                this.ServerPort = serverPort;
            }

            public byte Index { get; private set; }
            public string Name { get; set; }
            public string ServerName { get; set; }
            public string ServerIP { get; set; }
            public ushort ServerPort { get; set; }
        }
    }
}
