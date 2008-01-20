﻿/*
    Copyright 2007, Joe Davidson <joedavidson@gmail.com>

    This file is part of FFTPatcher.

    FFTPatcher is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FFTPatcher is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FFTPatcher.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;

namespace FFTPatcher.Datatypes
{
    public enum PreRequisiteJob
    {
        Base = 0x00,
        Chemist = 0x01,
        Knight = 0x02,
        Archer = 0x03,
        Monk = 0x04,
        WhiteMage = 0x05,
        BlackMage = 0x06,
        TimeMage = 0x07,
        Summoner = 0x08,
        Thief = 0x09,
        Mediator = 0x0A,
        Oracle = 0x0B,
        Geomancer = 0x0C,
        Lancer = 0x0D,
        Samurai = 0x0E,
        Ninja = 0x0F,
        Calculator = 0x10,
        Bard = 0x11,
        Dancer = 0x12,
        Mime = 0x13,
        DarkKnight = 0x14,
        OnionKnight = 0x15,
        Unknown = 0xA9,
    }

    public enum Facing
    {
        Southeast,
        Southwest,
        Northwest,
        Northeast,
        Unknown0x82 = 130,
        Unknown0x80 = 128,
        Unknown0x83 = 131,
        Unknown0x30 = 48,
        Unknown0x33 = 51,
        Unknown0x81 = 129,
    }

    /// <summary>
    /// Represents a unit that participates in an <see cref="Event"/>.
    /// </summary>
    public class EventUnit : IEquatable<EventUnit>
    {
        public SpriteSet SpriteSet { get; set; }

        public static readonly string[] Flags1FieldNames = new string[] { 
            "Male", "Female", "Monster", "JoinAfterEvent", "LoadFormation", "Blank1", "Blank2", "SaveFormation" };
        public static readonly string[] Flags2FieldNames = new string[] { 
            "Blank3", "Blank4", "Blank5", "Enemy", "Control", "Immortal", "Blank6", "Blank7" };

        public bool Male;
        public bool Female;
        public bool Monster;
        public bool JoinAfterEvent;
        public bool LoadFormation;
        public bool Blank1;
        public bool Blank2;
        public bool SaveFormation;

        public SpecialName SpecialName;
        public byte Level { get; set; }
        public Month Month { get; set; }
        public byte Day { get; set; }
        public byte Bravery { get; set; }
        public byte Faith { get; set; }
        public Job Job { get; set; }
        public SkillSet SecondaryAction { get; set; }
        public Ability Reaction { get; set; }
        public Ability Support { get; set; }
        public Ability Movement { get; set; }
        public Item Head { get; set; }
        public Item Body { get; set; }
        public Item Accessory { get; set; }
        public Item RightHand { get; set; }
        public Item LeftHand { get; set; }
        public byte Palette { get; set; }

        public bool Blank3;
        public bool Blank4;
        public bool Blank5;
        public bool Enemy;
        public bool Control;
        public bool Immortal;
        public bool Blank6;
        public bool Blank7;

        public byte X { get; set; }
        public byte Y { get; set; }

        public Facing FacingDirection { get; set; }
        public byte Unknown2 { get; set; }
        public SkillSet SkillSet { get; set; }
        public byte Unknown3 { get; set; }
        public byte Unknown4 { get; set; }
        public byte UnitID { get; set; }
        public byte Unknown6 { get; set; }
        public byte Unknown7 { get; set; }
        public byte Unknown8 { get; set; }
        public byte Unknown9 { get; set; }
        public byte Unknown10 { get; set; }
        public byte Unknown11 { get; set; }
        public byte Unknown12 { get; set; }

        public PreRequisiteJob PrerequisiteJob { get; set; }
        public byte PrerequisiteJobLevel { get; set; }

        public EventUnit Default { get; private set; }

        public string Description
        {
            get { return string.Format( "Sprite: {0} | Name: {1} | Job: {2}", SpriteSet.Name, SpecialName.Name, Job.Name ); }
        }

        public EventUnit( SubArray<byte> bytes, EventUnit defaults )
        {
            SpriteSet = SpriteSet.SpriteSets[bytes[0]];
            Default = defaults;
            Utilities.CopyByteToBooleans( bytes[1], ref Male, ref Female, ref Monster, ref JoinAfterEvent, ref LoadFormation, ref Blank1, ref Blank2, ref SaveFormation );
            SpecialName = SpecialName.SpecialNames[bytes[2]];
            Level = bytes[3];
            Month = (Month)bytes[4];
            Day = bytes[5];
            Bravery = bytes[6];
            Faith = bytes[7];
            PrerequisiteJob = (PreRequisiteJob)bytes[8];
            PrerequisiteJobLevel = bytes[9];
            Job = AllJobs.DummyJobs[bytes[10]];
            SecondaryAction = SkillSet.EventSkillSets[bytes[11]];
            Reaction = AllAbilities.EventAbilities[Utilities.BytesToUShort( bytes[12], bytes[13] )];
            Support = AllAbilities.EventAbilities[Utilities.BytesToUShort( bytes[14], bytes[15] )];
            Movement = AllAbilities.EventAbilities[Utilities.BytesToUShort( bytes[16], bytes[17] )];
            Head = Item.EventItems[bytes[18]];
            Body = Item.EventItems[bytes[19]];
            Accessory = Item.EventItems[bytes[20]];
            RightHand = Item.EventItems[bytes[21]];
            LeftHand = Item.EventItems[bytes[22]];
            Palette = bytes[23];
            Utilities.CopyByteToBooleans( bytes[24], ref Blank3, ref Blank4, ref Blank5, ref Enemy, ref Control, ref Immortal, ref Blank6, ref Blank7 );
            X = bytes[25];
            Y = bytes[26];
            FacingDirection = (Facing)bytes[27];
            Unknown2 = bytes[28];
            SkillSet = SkillSet.EventSkillSets[bytes[29]];
            Unknown3 = bytes[30];
            Unknown4 = bytes[31];
            UnitID = bytes[32];
            Unknown6 = bytes[33];
            Unknown7 = bytes[34];
            Unknown8 = bytes[35];
            Unknown9 = bytes[36];
            Unknown10 = bytes[37];
            Unknown11 = bytes[38];
            Unknown12 = bytes[39];
        }

        private static Dictionary<byte, object> b = new Dictionary<byte, object>();

        public EventUnit( SubArray<byte> bytes )
            : this( bytes, null )
        {
        }

        public byte[] ToByteArray()
        {
            List<byte> result = new List<byte>( 40 );
            result.Add( SpriteSet.ToByte() );
            result.Add( Utilities.ByteFromBooleans( Male, Female, Monster, JoinAfterEvent, LoadFormation, Blank1, Blank2, SaveFormation ) );
            result.Add( SpecialName.ToByte() );
            result.Add( Level );
            result.Add( (byte)Month );
            result.Add( Day );
            result.Add( Bravery );
            result.Add( Faith );
            result.Add( (byte)PrerequisiteJob );
            result.Add( PrerequisiteJobLevel );
            result.Add( Job.Value );
            result.Add( SecondaryAction.Value );
            result.AddRange( Reaction.Offset.ToBytes() );
            result.AddRange( Support.Offset.ToBytes() );
            result.AddRange( Movement.Offset.ToBytes() );
            result.Add( (byte)(Head.Offset & 0xFF) );
            result.Add( (byte)(Body.Offset & 0xFF) );
            result.Add( (byte)(Accessory.Offset & 0xFF) );
            result.Add( (byte)(RightHand.Offset & 0xFF) );
            result.Add( (byte)(LeftHand.Offset & 0xFF) );
            result.Add( Palette );
            result.Add( Utilities.ByteFromBooleans( Blank3, Blank4, Blank5, Enemy, Control, Immortal, Blank6, Blank7 ) );
            result.Add( X );
            result.Add( Y );
            result.Add( (byte)FacingDirection );
            result.Add( Unknown2 );
            result.Add( SkillSet.Value );
            result.Add( Unknown3 );
            result.Add( Unknown4 );
            result.Add( UnitID );
            result.Add( Unknown6 );
            result.Add( Unknown7 );
            result.Add( Unknown8 );
            result.Add( Unknown9 );
            result.Add( Unknown10 );
            result.Add( Unknown11 );
            result.Add( Unknown12 );

            return result.ToArray();
        }

        public override string ToString()
        {
            return Description;
        }

        public bool Equals( EventUnit other )
        {
            return Utilities.CompareArrays( other.ToByteArray(), this.ToByteArray() );
        }
    }
}