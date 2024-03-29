/*
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
using System.IO;
using PatcherLib.Datatypes;
using PatcherLib.Utilities;

namespace PatcherLib.Iso
{
    public static class PspIso
    {
        public class PspIsoInfo
        {
            private delegate void MyFunc( string path, Sectors sector );

            private IDictionary<Sectors, long> fileToSectorMap;
            private PspIsoInfo() { }
            public long this[PspIso.Sectors file] { get { return fileToSectorMap[file]; } }

            public static PspIsoInfo GetPspIsoInfo(Stream iso)
            {
                return GetPspIsoInfo(ImageMaster.IsoReader.GetRecord(iso));
            }

            private static PspIsoInfo GetPspIsoInfo( ImageMaster.ImageRecord record )
            {
                ImageMaster.ImageRecord myRecord = null;
                var myDict = new Dictionary<Sectors, long>();
                MyFunc func =
                    delegate( string path, Sectors sector )
                    {
                        myRecord = record.GetItemPath( path );
                        if (myRecord != null)
                        {
                            myDict[sector] = myRecord.Location;
                        }
                        else
                        {
                            throw new FileNotFoundException( "couldn't find file in ISO", path );
                        }
                    };
                func( "PSP_GAME/ICON0.PNG", Sectors.PSP_GAME_ICON0_PNG );
                func( "PSP_GAME/PARAM.SFO", Sectors.PSP_GAME_PARAM_SFO );
                func( "PSP_GAME/PIC0.PNG", Sectors.PSP_GAME_PIC0_PNG );
                func( "PSP_GAME/PIC1.PNG", Sectors.PSP_GAME_PIC1_PNG );
                func( "PSP_GAME/SYSDIR/BOOT.BIN", Sectors.PSP_GAME_SYSDIR_BOOT_BIN );
                func( "PSP_GAME/SYSDIR/EBOOT.BIN", Sectors.PSP_GAME_SYSDIR_EBOOT_BIN );
                func( "PSP_GAME/SYSDIR/UPDATE/DATA.BIN", Sectors.PSP_GAME_SYSDIR_UPDATE_DATA_BIN );
                func( "PSP_GAME/SYSDIR/UPDATE/EBOOT.BIN", Sectors.PSP_GAME_SYSDIR_UPDATE_EBOOT_BIN );
                func( "PSP_GAME/SYSDIR/UPDATE/PARAM.SFO", Sectors.PSP_GAME_SYSDIR_UPDATE_PARAM_SFO );
                func( "PSP_GAME/USRDIR/fftpack.bin", Sectors.PSP_GAME_USRDIR_fftpack_bin );
                func( "PSP_GAME/USRDIR/movie/001_HolyStone.pmf", Sectors.PSP_GAME_USRDIR_movie_001_HolyStone_pmf );
                func( "PSP_GAME/USRDIR/movie/002_Opening.pmf", Sectors.PSP_GAME_USRDIR_movie_002_Opening_pmf );
                func( "PSP_GAME/USRDIR/movie/003_Abduction.pmf", Sectors.PSP_GAME_USRDIR_movie_003_Abduction_pmf );
                func( "PSP_GAME/USRDIR/movie/004_Kusabue.pmf", Sectors.PSP_GAME_USRDIR_movie_004_Kusabue_pmf );
                func( "PSP_GAME/USRDIR/movie/005_Get_away.pmf", Sectors.PSP_GAME_USRDIR_movie_005_Get_away_pmf );
                func( "PSP_GAME/USRDIR/movie/006_Reassume_Dilita.pmf", Sectors.PSP_GAME_USRDIR_movie_006_Reassume_Dilita_pmf );
                func( "PSP_GAME/USRDIR/movie/007_Dilita_Advice.pmf", Sectors.PSP_GAME_USRDIR_movie_007_Dilita_Advice_pmf );
                func( "PSP_GAME/USRDIR/movie/008_Ovelia_and_Dilita.pmf", Sectors.PSP_GAME_USRDIR_movie_008_Ovelia_and_Dilita_pmf );
                func( "PSP_GAME/USRDIR/movie/009_Dilita_Musing.pmf", Sectors.PSP_GAME_USRDIR_movie_009_Dilita_Musing_pmf );
                func( "PSP_GAME/USRDIR/movie/010_Ending.pmf", Sectors.PSP_GAME_USRDIR_movie_010_Ending_pmf );
                func( "PSP_GAME/USRDIR/movie/011_Russo.pmf", Sectors.PSP_GAME_USRDIR_movie_011_Russo_pmf );
                func( "PSP_GAME/USRDIR/movie/012_Valuhurea.pmf", Sectors.PSP_GAME_USRDIR_movie_012_Valuhurea_pmf );
                func( "PSP_GAME/USRDIR/movie/013_StaffRoll.pmf", Sectors.PSP_GAME_USRDIR_movie_013_StaffRoll_pmf );
                func( "UMD_DATA.BIN", Sectors.UMD_DATA_BIN );
                PspIsoInfo result = new PspIsoInfo();
                result.fileToSectorMap = myDict;
                return result;
            }


            public static PspIsoInfo GetPspIsoInfo(string iso)
            {
                return GetPspIsoInfo(ImageMaster.IsoReader.GetRecord(iso));
            }
        }
        

		#region Instance Variables (6) 

        private static readonly long[] bootBinLocations = { 0x10000, 0x0FED8000 };
        private static byte[] buffer = new byte[1024];
        private const int bufferSize = 1024;
        private static byte[] euSizes = new byte[] { 0xA4, 0x84, 0x3A, 0x00, 0x00, 0x3A, 0x84, 0xA4 };
        //public const long FFTPackLocation = 0x02C20000;
        private static byte[] jpSizes = new byte[] { 0xE4, 0xD9, 0x37, 0x00, 0x00, 0x37, 0xD9, 0xE4 };

		#endregion Instance Variables 

		#region Public Methods (10) 

        /// <summary>
        /// Decrypts the ISO.
        /// </summary>
        /// <param name="filename">The filename of the ISO to decrypt.</param>
        public static void DecryptISO( string filename )
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream( filename, FileMode.Open );
                PspIsoInfo info = PspIsoInfo.GetPspIsoInfo( stream );
                DecryptISO( stream, info );
            }
            catch ( NotSupportedException )
            {
                throw;
            }
            finally
            {
                if ( stream != null )
                {
                    stream.Flush();
                    stream.Close();
                    stream = null;
                }
            }
        }

        /// <summary>
        /// Decrypts the ISO.
        /// </summary>
        /// <param name="stream">The stream of the ISO to decrypt.</param>
        public static void DecryptISO( Stream stream, PspIsoInfo info )
        {
            if ( IsJP( stream, info ) )
            {
                CopyBytes( stream, info[Sectors.PSP_GAME_SYSDIR_BOOT_BIN] * 2048, 0x37D9E4, info[Sectors.PSP_GAME_SYSDIR_EBOOT_BIN] * 2048, 0x37DB40 );
            }
            else if ( IsUS( stream, info ) || IsEU( stream, info ) )
            {
                CopyBytes( stream, info[Sectors.PSP_GAME_SYSDIR_BOOT_BIN] * 2048, 0x3A84A4, info[Sectors.PSP_GAME_SYSDIR_EBOOT_BIN] * 2048, 0x3A8600 );
            }
            else
            {
                throw new NotSupportedException( "Unrecognized image." );
            }
        }

        /// <summary>
        /// Determines whether the specified stream is EU.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// 	<c>true</c> if the specified stream is EU; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEU( Stream stream, PspIsoInfo info )
        {
            return
                CheckString( stream, info[Sectors.UMD_DATA_BIN] * 2048 + 0, "ULES-00850" ) &&
                CheckString( stream, info[Sectors.PSP_GAME_PARAM_SFO] * 2048 + 0x128, "ULES00850" ) &&
                CheckString( stream, info[Sectors.PSP_GAME_SYSDIR_BOOT_BIN] * 2048 + 0x3143A8, "ULES00850" ) &&
                CheckString( stream, info[Sectors.PSP_GAME_SYSDIR_BOOT_BIN] * 2048 + 0x35A530, "ULES00850" );
        }

        /// <summary>
        /// Determines whether the specified stream is JP.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// 	<c>true</c> if the specified stream is JP; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJP( Stream stream, PspIsoInfo info )
        {
            //return CheckFile( stream, "ULJM-05194", "ULJM05194", new long[] { 0x8373, 0xE000 }, new long[] { 0x2BF0128, 0xFD619FC, 0xFD97A5C } );
            return
                CheckString( stream, info[Sectors.UMD_DATA_BIN] * 2048 + 0, "ULJM-05194" );
        }

        /// <summary>
        /// Determines whether the specified stream is US.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// 	<c>true</c> if the specified stream is US; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUS( Stream stream, PspIsoInfo info )
        {
            return
                CheckString( stream, info[Sectors.UMD_DATA_BIN] * 2048 + 0, "ULUS-10297" ) &&
                CheckString( stream, info[Sectors.PSP_GAME_PARAM_SFO] * 2048 + 0x128, "ULUS10297" ) &&
                CheckString( stream, info[Sectors.PSP_GAME_SYSDIR_BOOT_BIN] * 2048 + 0x3143A8, "ULUS10297" ) &&
                CheckString( stream, info[Sectors.PSP_GAME_SYSDIR_BOOT_BIN] * 2048 + 0x35A530, "ULUS10297" );
        }

        private static bool CheckString( Stream stream, long loc, string expectedString )
        {
            stream.Seek( loc, SeekOrigin.Begin );
            byte[] buffer = new byte[expectedString.Length];
            stream.Read( buffer, 0, buffer.Length );
            return buffer.ToUTF8String() == expectedString;
        }

        public static void PatchISO( Stream file, IEnumerable<PatcherLib.Datatypes.PatchedByteArray> patches )
        {
            PspIsoInfo info = PspIsoInfo.GetPspIsoInfo( file );
            DecryptISO( file, info );
            patches.ForEach( p => ApplyPatch( file, info, p ) );
        }

		#endregion Public Methods 

		#region Private Methods (3) 

        public static void ApplyPatch( Stream stream, PspIsoInfo info, PatcherLib.Datatypes.PatchedByteArray patch )
        {
            if ( patch.SectorEnum != null )
            {
                if ( patch.SectorEnum.GetType() == typeof( PspIso.Sectors ) )
                {
                    stream.WriteArrayToPosition( patch.Bytes, (int)( info[(PspIso.Sectors)patch.SectorEnum] * 2048 ) + patch.Offset );
                }
                else if ( patch.SectorEnum.GetType() == typeof( FFTPack.Files ) )
                {
                    FFTPack.PatchFile( stream, info, (int)( (FFTPack.Files)patch.SectorEnum ), (int)patch.Offset, patch.Bytes );
                }
                else
                {
                    throw new ArgumentException( "invalid type" );
                }
            }
        }

        public static IList<byte> GetFile( Stream stream, PspIsoInfo info, PspIso.Sectors sector, int start, int length )
        {
            byte[] result = new byte[length];
            stream.Seek( info[sector] * 2048 + start, SeekOrigin.Begin );
            stream.Read( result, 0, length );
            return result;
        }

        public static IList<byte> GetFile( Stream stream, PspIsoInfo info, FFTPack.Files file, int start, int length )
        {
            byte[] result = FFTPack.GetFileFromIso( stream, info, file );
            return result.Sub( start, start + length - 1 );
        }

        public static IList<byte> GetBlock(Stream iso, PspIsoInfo info, KnownPosition pos)
        {
            if (pos.FFTPack.HasValue)
            {
                return GetFile(iso, info, pos.FFTPack.Value, pos.StartLocation, pos.Length);
            }
            else if (pos.Sector.HasValue)
            {
                return GetFile(iso, info, pos.Sector.Value, pos.StartLocation, pos.Length);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private static void CopyBytes( Stream stream, long src, long srcSize, long dest, long destOldSize )
        {
            long bytesRead = 0;
            while ( ( bytesRead + bufferSize ) < srcSize )
            {
                stream.Seek( src + bytesRead, SeekOrigin.Begin );
                stream.Read( buffer, 0, bufferSize );
                stream.Seek( dest + bytesRead, SeekOrigin.Begin );
                stream.Write( buffer, 0, bufferSize );
                bytesRead += bufferSize;
            }

            stream.Seek( src + bytesRead, SeekOrigin.Begin );
            stream.Read( buffer, 0, (int)( srcSize - bytesRead ) );
            stream.Seek( dest + bytesRead, SeekOrigin.Begin );
            stream.Write( buffer, 0, (int)( srcSize - bytesRead ) );

            if ( destOldSize > srcSize )
            {
                buffer = new byte[bufferSize];
                stream.Seek( dest + srcSize, SeekOrigin.Begin );
                stream.Write( buffer, 0, (int)( destOldSize - srcSize ) );
            }
        }

        public class KnownPosition
        {
            public Enum SectorEnum { get; private set; }
            public Sectors? Sector { get; private set; }
            public FFTPack.Files? FFTPack { get; private set; }

            public int StartLocation { get; private set; }
            public int Length { get; private set; }

            private KnownPosition(Enum sector, int startLocation, int length)
            {
                SectorEnum = sector;
                StartLocation = startLocation;
                Length = length;
            }

            public KnownPosition(Sectors sector, int startLocation, int length)
                : this((Enum)sector, startLocation, length)
            {
                Sector = sector;
            }

            public KnownPosition(FFTPack.Files sector, int startLocation, int length)
                : this((Enum)sector, startLocation, length)
            {
                FFTPack = sector;
            }

            public PatchedByteArray GetPatchedByteArray(byte[] bytes)
            {
                if (Sector.HasValue)
                {
                    return new PatchedByteArray(Sector, StartLocation, bytes);
                }
                else if (FFTPack.HasValue)
                {
                    return new PatchedByteArray(FFTPack, StartLocation, bytes);
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public static IList<KnownPosition> Abilities { get; private set; }

        public static IList<KnownPosition> AbilityEffects { get; private set; }

        public static IList<KnownPosition> ActionEvents { get; private set; }

        public static KnownPosition ENTD1 { get; private set; }

        public static KnownPosition ENTD2 { get; private set; }

        public static KnownPosition ENTD3 { get; private set; }

        public static KnownPosition ENTD4 { get; private set; }
        public static KnownPosition ENTD5 { get; private set; }

        public static IList<KnownPosition> InflictStatuses { get; private set; }

        public static IList<KnownPosition> JobLevels { get; private set; }

        public static IList<KnownPosition> Jobs { get; private set; }

        public static IList<KnownPosition> MonsterSkills { get; private set; }

        public static IList<KnownPosition> MoveFindItems { get; private set; }

        public static IList<KnownPosition> OldItemAttributes { get; private set; }

        public static IList<KnownPosition> OldItems { get; private set; }

        public static IList<KnownPosition> NewItemAttributes { get; private set; }

        public static IList<KnownPosition> NewItems { get; private set; }

        public static IList<KnownPosition> PoachProbabilities { get; private set; }

        public static IList<KnownPosition> SkillSets { get; private set; }

        public static IList<KnownPosition> StatusAttributes { get; private set; }

        public static IList<KnownPosition> StoreInventories { get; private set; }


        static PspIso()
        {
            Abilities = new KnownPosition[] { 
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x271514, 0x24C6),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x271514, 0x24C6) }.AsReadOnly();
            AbilityEffects = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x3177B4, 0x2E0),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x3177B4, 0x2E0)}.AsReadOnly();
            ActionEvents = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x276CA4, 227),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x276CA4, 227)}.AsReadOnly();
            InflictStatuses = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x3263E8, 0x300),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x3263E8, 0x300)}.AsReadOnly();
            Jobs = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x2739DC, 8281),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x2739DC, 8281)}.AsReadOnly();
            JobLevels = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x277084, 280),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x277084, 280)}.AsReadOnly();
            MonsterSkills = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x276BB4, 0xF0),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x276BB4, 0xF0)}.AsReadOnly();
            OldItemAttributes = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x3266E8, 0x7D0),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x3266E8, 0x7D0)}.AsReadOnly();
            NewItemAttributes = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x25720C, 0x20D),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x25720C, 0x20D)}.AsReadOnly();

            OldItems = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x3252DC, 0x110A),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x3252DC, 0x110A)}.AsReadOnly();
            NewItems = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x256E00, 1032),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x256E00, 1032)}.AsReadOnly();
            PoachProbabilities = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x277024, 0x60),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x277024, 0x60)}.AsReadOnly();
            StatusAttributes = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x276DA4, 0x280),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x276DA4, 0x280)}.AsReadOnly();

            SkillSets = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x275A38, 4475),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x275A38, 4475)}.AsReadOnly();

            MoveFindItems = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x2707A8, 0x800),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x2707A8, 0x800)}.AsReadOnly();

            StoreInventories = new KnownPosition[] {
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_BOOT_BIN, 0x2DC8D0, 0x200),
                new KnownPosition(Sectors.PSP_GAME_SYSDIR_EBOOT_BIN, 0x2DC8D0, 0x200)}.AsReadOnly();

            ENTD1 = new KnownPosition(FFTPack.Files.BATTLE_ENTD1_ENT, 0, 81920);
            ENTD2 = new KnownPosition(FFTPack.Files.BATTLE_ENTD2_ENT, 0, 81920);
            ENTD3 = new KnownPosition(FFTPack.Files.BATTLE_ENTD3_ENT, 0, 81920);
            ENTD4 = new KnownPosition(FFTPack.Files.BATTLE_ENTD4_ENT, 0, 81920);
            ENTD5 = new KnownPosition(FFTPack.Files.BATTLE_ENTD5_ENT, 0, 51200);
        }

		#endregion Private Methods 

        public enum Sectors
        {
            PSP_GAME_ICON0_PNG = 22560,
            PSP_GAME_PARAM_SFO = 22576,
            PSP_GAME_PIC0_PNG = 22416,
            PSP_GAME_PIC1_PNG = 22432,
            PSP_GAME_SYSDIR_BOOT_BIN = 130480,
            PSP_GAME_SYSDIR_EBOOT_BIN = 32,
            PSP_GAME_SYSDIR_UPDATE_DATA_BIN = 6032,
            PSP_GAME_SYSDIR_UPDATE_EBOOT_BIN = 1936,
            PSP_GAME_SYSDIR_UPDATE_PARAM_SFO = 1920,
            PSP_GAME_USRDIR_fftpack_bin = 22592,
            PSP_GAME_USRDIR_movie_001_HolyStone_pmf = 132368,
            PSP_GAME_USRDIR_movie_002_Opening_pmf = 190832,
            PSP_GAME_USRDIR_movie_003_Abduction_pmf = 198112,
            PSP_GAME_USRDIR_movie_004_Kusabue_pmf = 135360,
            PSP_GAME_USRDIR_movie_005_Get_away_pmf = 140288,
            PSP_GAME_USRDIR_movie_006_Reassume_Dilita_pmf = 144352,
            PSP_GAME_USRDIR_movie_007_Dilita_Advice_pmf = 150224,
            PSP_GAME_USRDIR_movie_008_Ovelia_and_Dilita_pmf = 156000,
            PSP_GAME_USRDIR_movie_009_Dilita_Musing_pmf = 166192,
            PSP_GAME_USRDIR_movie_010_Ending_pmf = 179264,
            PSP_GAME_USRDIR_movie_011_Russo_pmf = 183360,
            PSP_GAME_USRDIR_movie_012_Valuhurea_pmf = 186304,
            PSP_GAME_USRDIR_movie_013_StaffRoll_pmf = 202128,
            UMD_DATA_BIN = 28,
        }
    }
}
