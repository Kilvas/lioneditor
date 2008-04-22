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

using System.Collections.Generic;

namespace FFTPatcher.Datatypes
{
    /// <summary>
    /// Represents the JP needed to grow in a job level as well as the prerequisites for unlocking jobs.
    /// </summary>
    public class JobLevels : IChangeable
    {

		#region Fields (1) 

        private string[] reqs = new string[] {
            "Chemist", "Knight", "Archer", "Monk", "WhiteMage", "BlackMage", "TimeMage", "Summoner", "Thief", "Orator", 
            "Mystic", "Geomancer", "Dragoon", "Samurai", "Ninja", "Arithmetician", "Bard", "Dancer", "Mime", "DarkKnight", "OnionKnight", "Unknown" };

		#endregion Fields 

		#region Properties (32) 


        public Requirements Archer { get; private set; }

        public Requirements Arithmetician { get; private set; }

        public Requirements Bard { get; private set; }

        public Requirements BlackMage { get; private set; }

        public Requirements Chemist { get; private set; }

        public Requirements Dancer { get; private set; }

        public Requirements DarkKnight { get; private set; }

        public JobLevels Default { get; private set; }

        public Requirements Dragoon { get; private set; }

        public Requirements Geomancer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has changed.
        /// </summary>
        /// <value></value>
        public bool HasChanged
        {
            get
            {
                return (Default != null) &&
                    Level1 != Default.Level1 ||
                    Level2 != Default.Level2 ||
                    Level3 != Default.Level3 ||
                    Level4 != Default.Level4 ||
                    Level5 != Default.Level5 ||
                    Level6 != Default.Level6 ||
                    Level7 != Default.Level7 ||
                    Level8 != Default.Level8 ||
                    Archer.HasChanged ||
                    Arithmetician.HasChanged ||
                    Bard.HasChanged ||
                    BlackMage.HasChanged ||
                    Chemist.HasChanged ||
                    Dancer.HasChanged ||
                    Dragoon.HasChanged ||
                    Geomancer.HasChanged ||
                    Knight.HasChanged ||
                    Mime.HasChanged ||
                    Monk.HasChanged ||
                    Mystic.HasChanged ||
                    Ninja.HasChanged ||
                    Orator.HasChanged ||
                    Samurai.HasChanged ||
                    Summoner.HasChanged ||
                    Thief.HasChanged ||
                    TimeMage.HasChanged ||
                    WhiteMage.HasChanged ||
                    (OnionKnight != null && OnionKnight.HasChanged) ||
                    (DarkKnight != null && DarkKnight.HasChanged) ||
                    (Unknown != null && Unknown.HasChanged);
            }
        }

        public Requirements Knight { get; private set; }

        public ushort Level1 { get; set; }

        public ushort Level2 { get; set; }

        public ushort Level3 { get; set; }

        public ushort Level4 { get; set; }

        public ushort Level5 { get; set; }

        public ushort Level6 { get; set; }

        public ushort Level7 { get; set; }

        public ushort Level8 { get; set; }

        public Requirements Mime { get; private set; }

        public Requirements Monk { get; private set; }

        public Requirements Mystic { get; private set; }

        public Requirements Ninja { get; private set; }

        public Requirements OnionKnight { get; private set; }

        public Requirements Orator { get; private set; }

        public Requirements Samurai { get; private set; }

        public Requirements Summoner { get; private set; }

        public Requirements Thief { get; private set; }

        public Requirements TimeMage { get; private set; }

        public Requirements Unknown { get; private set; }

        public Requirements WhiteMage { get; private set; }


		#endregion Properties 

		#region Constructors (3) 

        public JobLevels( IList<byte> bytes )
            : this( Context.US_PSP, bytes )
        {
        }

        public JobLevels( Context context, IList<byte> bytes )
            : this( context, bytes, null )
        {
        }

        public JobLevels( Context context, IList<byte> bytes, JobLevels defaults )
        {
            Default = defaults;
            int jobCount = context == Context.US_PSP ? 22 : 19;
            int requirementsLength = context == Context.US_PSP ? 12 : 10;

            for( int i = 0; i < jobCount; i++ )
            {
                ReflectionHelpers.SetFieldOrProperty( this, reqs[i],
                    new Requirements( context,
                        bytes.Sub( i * requirementsLength, (i + 1) * requirementsLength - 1 ),
                        defaults == null ? null : ReflectionHelpers.GetFieldOrProperty<Requirements>( defaults, reqs[i] ) ) );
            }

            int start = requirementsLength * jobCount;
            if( context == Context.US_PSX )
                start += 2;
            Level1 = Utilities.BytesToUShort( bytes[start + 0], bytes[start + 1] );
            Level2 = Utilities.BytesToUShort( bytes[start + 2], bytes[start + 3] );
            Level3 = Utilities.BytesToUShort( bytes[start + 4], bytes[start + 5] );
            Level4 = Utilities.BytesToUShort( bytes[start + 6], bytes[start + 7] );
            Level5 = Utilities.BytesToUShort( bytes[start + 8], bytes[start + 9] );
            Level6 = Utilities.BytesToUShort( bytes[start + 10], bytes[start + 11] );
            Level7 = Utilities.BytesToUShort( bytes[start + 12], bytes[start + 13] );
            Level8 = Utilities.BytesToUShort( bytes[start + 14], bytes[start + 15] );
        }

		#endregion Constructors 

		#region Methods (3) 


        public List<string> GenerateCodes()
        {
            if( FFTPatch.Context == Context.US_PSP )
            {
                return Codes.GenerateCodes( Context.US_PSP, Resources.JobLevelsBin, this.ToByteArray(), 0x27B030 );
            }
            else
            {
                return Codes.GenerateCodes( Context.US_PSX, PSXResources.JobLevelsBin, this.ToByteArray( Context.US_PSX ), 0x0660C4 );
            }
        }

        public byte[] ToByteArray()
        {
            return ToByteArray( Context.US_PSP );
        }

        public byte[] ToByteArray( Context context )
        {
            int jobCount = context == Context.US_PSP ? 22 : 19;
            List<byte> result = new List<byte>( 0x118 );
            for( int i = 0; i < jobCount; i++ )
            {
                result.AddRange( ReflectionHelpers.GetFieldOrProperty<Requirements>( this, reqs[i] ).ToByteArray( context ) );
            }
            if( context == Context.US_PSX )
            {
                result.Add( 0x00 );
                result.Add( 0x00 );
            }
            result.AddRange( Level1.ToBytes() );
            result.AddRange( Level2.ToBytes() );
            result.AddRange( Level3.ToBytes() );
            result.AddRange( Level4.ToBytes() );
            result.AddRange( Level5.ToBytes() );
            result.AddRange( Level6.ToBytes() );
            result.AddRange( Level7.ToBytes() );
            result.AddRange( Level8.ToBytes() );

            return result.ToArray();
        }


		#endregion Methods 

    }

    /// <summary>
    /// Represents the prerequisites to unlock a job.
    /// </summary>
    public class Requirements : IChangeable
    {

		#region Properties (26) 


        public int Archer { get; set; }

        public int Arithmetician { get; set; }

        public int Bard { get; set; }

        public int BlackMage { get; set; }

        public int Chemist { get; set; }

        public int Dancer { get; set; }

        public int DarkKnight { get; set; }

        public Requirements Default { get; private set; }

        public int Dragoon { get; set; }

        public int Geomancer { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has changed.
        /// </summary>
        /// <value></value>
        public bool HasChanged
        {
            get 
            {
                return
                    Default != null &&
                    !Utilities.CompareArrays( ToByteArray( FFTPatch.Context ), Default.ToByteArray( FFTPatch.Context ) ); 
            }
        }

        public int Knight { get; set; }

        public int Mime { get; set; }

        public int Monk { get; set; }

        public int Mystic { get; set; }

        public int Ninja { get; set; }

        public int OnionKnight { get; set; }

        public int Orator { get; set; }

        public int Samurai { get; set; }

        public int Squire { get; set; }

        public int Summoner { get; set; }

        public int Thief { get; set; }

        public int TimeMage { get; set; }

        public int Unknown1 { get; set; }

        public int Unknown2 { get; set; }

        public int WhiteMage { get; set; }


		#endregion Properties 

		#region Constructors (3) 

        public Requirements( IList<byte> bytes )
            : this( Context.US_PSP, bytes )
        {
        }

        public Requirements( Context context, IList<byte> bytes )
            : this( context, bytes, null )
        {
        }

        public Requirements( Context context, IList<byte> bytes, Requirements defaults )
        {
            Default = defaults;
            Squire = bytes[0].GetUpperNibble();
            Chemist = bytes[0].GetLowerNibble();
            Knight = bytes[1].GetUpperNibble();
            Archer = bytes[1].GetLowerNibble();
            Monk = bytes[2].GetUpperNibble();
            WhiteMage = bytes[2].GetLowerNibble();
            BlackMage = bytes[3].GetUpperNibble();
            TimeMage = bytes[3].GetLowerNibble();
            Summoner = bytes[4].GetUpperNibble();
            Thief = bytes[4].GetLowerNibble();
            Orator = bytes[5].GetUpperNibble();
            Mystic = bytes[5].GetLowerNibble();
            Geomancer = bytes[6].GetUpperNibble();
            Dragoon = bytes[6].GetLowerNibble();
            Samurai = bytes[7].GetUpperNibble();
            Ninja = bytes[7].GetLowerNibble();
            Arithmetician = bytes[8].GetUpperNibble();
            Bard = bytes[8].GetLowerNibble();
            Dancer = bytes[9].GetUpperNibble();
            Mime = bytes[9].GetLowerNibble();
            if( context == Context.US_PSP )
            {
                DarkKnight = bytes[10].GetUpperNibble();
                OnionKnight = bytes[10].GetLowerNibble();
                Unknown1 = bytes[11].GetUpperNibble();
                Unknown2 = bytes[11].GetLowerNibble();
            }
        }

		#endregion Constructors 

		#region Methods (2) 


        public byte[] ToByteArray( Context context )
        {
            List<byte> result = new List<byte>( 12 );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( Squire, Chemist ) );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( Knight, Archer ) );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( Monk, WhiteMage ) );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( BlackMage, TimeMage ) );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( Summoner, Thief ) );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( Orator, Mystic ) );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( Geomancer, Dragoon ) );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( Samurai, Ninja ) );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( Arithmetician, Bard ) );
            result.Add( Utilities.MoveToUpperAndLowerNibbles( Dancer, Mime ) );
            if( context == Context.US_PSP )
            {
                result.Add( Utilities.MoveToUpperAndLowerNibbles( DarkKnight, OnionKnight ) );
                result.Add( Utilities.MoveToUpperAndLowerNibbles( Unknown1, Unknown2 ) );
            }

            return result.ToArray();
        }

        public byte[] ToByteArray()
        {
            return ToByteArray( Context.US_PSP );
        }


		#endregion Methods 

    }
}