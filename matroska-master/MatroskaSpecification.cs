using System.Collections.Generic;
using System.Reflection;

namespace Matroska
{
	/// <summary>
	/// Contains the EBML elements specified by the Matroska project.
	/// </summary>
	/// <remarks>
	/// See https://matroska.org/technical/specs/index.html for more info.
	/// </remarks>
	public static class MatroskaSpecification
	{
		#region Helper

		private static readonly Dictionary<uint, EbmlElementInfo> infos;

		/// <summary>
		/// Gets a dictionary of all Matroska elements.
		/// </summary>
		public static IReadOnlyDictionary<uint, EbmlElementInfo> Elements => infos;

		static MatroskaSpecification()
		{
			infos = new Dictionary<uint, EbmlElementInfo>();
			FieldInfo[] fields = typeof(MatroskaSpecification).GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo field in fields)
			{
				EbmlElementInfo info = (EbmlElementInfo)field.GetValue(null);
				infos.Add(info.ID, info);
			}
		}

		#endregion

		#region Definitions

		/// <summary>Set the EBML characteristics of the data to follow. Each EBML document has to start with this.</summary>
		public static readonly EbmlElementInfo EBML = new EbmlElementInfo(0x1A45DFA3, "EBML", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 0, "Set the EBML characteristics of the data to follow. Each EBML document has to start with this.");

		/// <summary>The version of EBML parser used to create the file.</summary>
		public static readonly EbmlElementInfo EBMLVersion = new EbmlElementInfo(0x00004286, "EBMLVersion", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 1, "The version of EBML parser used to create the file.");

		/// <summary>The minimum EBML version a parser has to support to read this file.</summary>
		public static readonly EbmlElementInfo EBMLReadVersion = new EbmlElementInfo(0x000042F7, "EBMLReadVersion", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 1, "The minimum EBML version a parser has to support to read this file.");

		/// <summary>The maximum length of the IDs you'll find in this file (4 or less in Matroska).</summary>
		public static readonly EbmlElementInfo EBMLMaxIDLength = new EbmlElementInfo(0x000042F2, "EBMLMaxIDLength", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 1, "The maximum length of the IDs you'll find in this file (4 or less in Matroska).");

		/// <summary>The maximum length of the sizes you'll find in this file (8 or less in Matroska). This does not override the Element size indicated at the beginning of an Element. Elements that have an indicated size which is larger than what is allowed by EBMLMaxSizeLength shall be considered invalid.</summary>
		public static readonly EbmlElementInfo EBMLMaxSizeLength = new EbmlElementInfo(0x000042F3, "EBMLMaxSizeLength", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 1, "The maximum length of the sizes you'll find in this file (8 or less in Matroska). This does not override the Element size indicated at the beginning of an Element. Elements that have an indicated size which is larger than what is allowed by EBMLMaxSizeLength shall be considered invalid.");

		/// <summary>A string that describes the type of document that follows this EBML header. 'matroska' in our case or 'webm' for webm files.</summary>
		public static readonly EbmlElementInfo DocType = new EbmlElementInfo(0x00004282, "DocType", EbmlElementType.String, EbmlElementFlags.Mandatory, 1, "A string that describes the type of document that follows this EBML header. 'matroska' in our case or 'webm' for webm files.");

		/// <summary>The version of DocType interpreter used to create the file.</summary>
		public static readonly EbmlElementInfo DocTypeVersion = new EbmlElementInfo(0x00004287, "DocTypeVersion", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 1, "The version of DocType interpreter used to create the file.");

		/// <summary>The minimum DocType version an interpreter has to support to read this file.</summary>
		public static readonly EbmlElementInfo DocTypeReadVersion = new EbmlElementInfo(0x00004285, "DocTypeReadVersion", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 1, "The minimum DocType version an interpreter has to support to read this file.");

		/// <summary>Used to void damaged data, to avoid unexpected behaviors when using damaged data. The content is discarded. Also used to reserve space in a sub-element for later use.</summary>
		public static readonly EbmlElementInfo Void = new EbmlElementInfo(0x000000EC, "Void", EbmlElementType.Binary, EbmlElementFlags.None, -1, "Used to void damaged data, to avoid unexpected behaviors when using damaged data. The content is discarded. Also used to reserve space in a sub-element for later use.");

		/// <summary>The CRC is computed on all the data of the Master-element it's in. The CRC Element should be the first in it's parent master for easier reading. All level 1 Elements should include a CRC-32. The CRC in use is the IEEE CRC32 Little Endian</summary>
		public static readonly EbmlElementInfo CRC32 = new EbmlElementInfo(0x000000BF, "CRC-32", EbmlElementType.Binary, EbmlElementFlags.None, -1, "The CRC is computed on all the data of the Master-element it's in. The CRC Element should be the first in it's parent master for easier reading. All level 1 Elements should include a CRC-32. The CRC in use is the IEEE CRC32 Little Endian");

		/// <summary>Contain signature of some (coming) Elements in the stream.</summary>
		public static readonly EbmlElementInfo SignatureSlot = new EbmlElementInfo(0x1B538667, "SignatureSlot", EbmlElementType.Master, EbmlElementFlags.None, -1, "Contain signature of some (coming) Elements in the stream.");

		/// <summary>Signature algorithm used (1=RSA, 2=elliptic).</summary>
		public static readonly EbmlElementInfo SignatureAlgo = new EbmlElementInfo(0x00007E8A, "SignatureAlgo", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 1, "Signature algorithm used (1=RSA, 2=elliptic).");

		/// <summary>Hash algorithm used (1=SHA1-160, 2=MD5).</summary>
		public static readonly EbmlElementInfo SignatureHash = new EbmlElementInfo(0x00007E9A, "SignatureHash", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 1, "Hash algorithm used (1=SHA1-160, 2=MD5).");

		/// <summary>The public key to use with the algorithm (in the case of a PKI-based signature).</summary>
		public static readonly EbmlElementInfo SignaturePublicKey = new EbmlElementInfo(0x00007EA5, "SignaturePublicKey", EbmlElementType.Binary, EbmlElementFlags.None, 1, "The public key to use with the algorithm (in the case of a PKI-based signature).");

		/// <summary>The signature of the data (until a new.</summary>
		public static readonly EbmlElementInfo Signature = new EbmlElementInfo(0x00007EB5, "Signature", EbmlElementType.Binary, EbmlElementFlags.None, 1, "The signature of the data (until a new.");

		/// <summary>Contains Elements that will be used to compute the signature.</summary>
		public static readonly EbmlElementInfo SignatureElements = new EbmlElementInfo(0x00007E5B, "SignatureElements", EbmlElementType.Master, EbmlElementFlags.None, 1, "Contains Elements that will be used to compute the signature.");

		/// <summary>A list consists of a number of consecutive Elements that represent one case where data is used in signature. Ex: Cluster|Block|BlockAdditional means that the BlockAdditional of all Blocks in all Clusters is used for encryption.</summary>
		public static readonly EbmlElementInfo SignatureElementList = new EbmlElementInfo(0x00007E7B, "SignatureElementList", EbmlElementType.Master, EbmlElementFlags.None, 2, "A list consists of a number of consecutive Elements that represent one case where data is used in signature. Ex: Cluster|Block|BlockAdditional means that the BlockAdditional of all Blocks in all Clusters is used for encryption.");

		/// <summary>An Element ID whose data will be used to compute the signature.</summary>
		public static readonly EbmlElementInfo SignedElement = new EbmlElementInfo(0x00006532, "SignedElement", EbmlElementType.Binary, EbmlElementFlags.None, 3, "An Element ID whose data will be used to compute the signature.");

		/// <summary>The Root Element that contains all other Top-Level Elements (Elements defined only at Level 1). A Matroska file is composed of 1 Segment.</summary>
		public static readonly EbmlElementInfo Segment = new EbmlElementInfo(0x18538067, "Segment", EbmlElementType.Master, EbmlElementFlags.Mandatory, 0, "The Root Element that contains all other Top-Level Elements (Elements defined only at Level 1). A Matroska file is composed of 1 Segment.");

		/// <summary>Contains the position of other Top-Level Elements.</summary>
		public static readonly EbmlElementInfo SeekHead = new EbmlElementInfo(0x114D9B74, "SeekHead", EbmlElementType.Master, EbmlElementFlags.None, 1, "Contains the position of other Top-Level Elements.");

		/// <summary>Contains a single seek entry to an EBML Element.</summary>
		public static readonly EbmlElementInfo Seek = new EbmlElementInfo(0x00004DBB, "Seek", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 2, "Contains a single seek entry to an EBML Element.");

		/// <summary>The binary ID corresponding to the Element name.</summary>
		public static readonly EbmlElementInfo SeekID = new EbmlElementInfo(0x000053AB, "SeekID", EbmlElementType.Binary, EbmlElementFlags.Mandatory, 3, "The binary ID corresponding to the Element name.");

		/// <summary>The position of the Element in the Segment in octets (0 = first level 1 Element).</summary>
		public static readonly EbmlElementInfo SeekPosition = new EbmlElementInfo(0x000053AC, "SeekPosition", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "The position of the Element in the Segment in octets (0 = first level 1 Element).");

		/// <summary>Contains miscellaneous general information and statistics on the file.</summary>
		public static readonly EbmlElementInfo Info = new EbmlElementInfo(0x1549A966, "Info", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 1, "Contains miscellaneous general information and statistics on the file.");

		/// <summary>A randomly generated unique ID to identify the current Segment between many others (128 bits).</summary>
		public static readonly EbmlElementInfo SegmentUID = new EbmlElementInfo(0x000073A4, "SegmentUID", EbmlElementType.Binary, EbmlElementFlags.None, 2, "A randomly generated unique ID to identify the current Segment between many others (128 bits).");

		/// <summary>A filename corresponding to this Segment.</summary>
		public static readonly EbmlElementInfo SegmentFilename = new EbmlElementInfo(0x00007384, "SegmentFilename", EbmlElementType.UTF8, EbmlElementFlags.None, 2, "A filename corresponding to this Segment.");

		/// <summary>A unique ID to identify the previous chained Segment (128 bits).</summary>
		public static readonly EbmlElementInfo PrevUID = new EbmlElementInfo(0x003CB923, "PrevUID", EbmlElementType.Binary, EbmlElementFlags.None, 2, "A unique ID to identify the previous chained Segment (128 bits).");

		/// <summary>An escaped filename corresponding to the previous Segment.</summary>
		public static readonly EbmlElementInfo PrevFilename = new EbmlElementInfo(0x003C83AB, "PrevFilename", EbmlElementType.UTF8, EbmlElementFlags.None, 2, "An escaped filename corresponding to the previous Segment.");

		/// <summary>A unique ID to identify the next chained Segment (128 bits).</summary>
		public static readonly EbmlElementInfo NextUID = new EbmlElementInfo(0x003EB923, "NextUID", EbmlElementType.Binary, EbmlElementFlags.None, 2, "A unique ID to identify the next chained Segment (128 bits).");

		/// <summary>An escaped filename corresponding to the next Segment.</summary>
		public static readonly EbmlElementInfo NextFilename = new EbmlElementInfo(0x003E83BB, "NextFilename", EbmlElementType.UTF8, EbmlElementFlags.None, 2, "An escaped filename corresponding to the next Segment.");

		/// <summary>A randomly generated unique ID that all Segments related to each other must use (128 bits).</summary>
		public static readonly EbmlElementInfo SegmentFamily = new EbmlElementInfo(0x00004444, "SegmentFamily", EbmlElementType.Binary, EbmlElementFlags.None, 2, "A randomly generated unique ID that all Segments related to each other must use (128 bits).");

		/// <summary>A tuple of corresponding ID used by chapter codecs to represent this Segment.</summary>
		public static readonly EbmlElementInfo ChapterTranslate = new EbmlElementInfo(0x00006924, "ChapterTranslate", EbmlElementType.Master, EbmlElementFlags.None, 2, "A tuple of corresponding ID used by chapter codecs to represent this Segment.");

		/// <summary>Specify an edition UID on which this correspondance applies. When not specified, it means for all editions found in the Segment.</summary>
		public static readonly EbmlElementInfo ChapterTranslateEditionUID = new EbmlElementInfo(0x000069FC, "ChapterTranslateEditionUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "Specify an edition UID on which this correspondance applies. When not specified, it means for all editions found in the Segment.");

		/// <summary>The chapter codec using this ID (0: Matroska Script, 1: DVD-menu).</summary>
		public static readonly EbmlElementInfo ChapterTranslateCodec = new EbmlElementInfo(0x000069BF, "ChapterTranslateCodec", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "The chapter codec using this ID (0: Matroska Script, 1: DVD-menu).");

		/// <summary>The binary value used to represent this Segment in the chapter codec data. The format depends on the ChapProcessCodecID used.</summary>
		public static readonly EbmlElementInfo ChapterTranslateID = new EbmlElementInfo(0x000069A5, "ChapterTranslateID", EbmlElementType.Binary, EbmlElementFlags.Mandatory, 3, "The binary value used to represent this Segment in the chapter codec data. The format depends on the ChapProcessCodecID used.");

		/// <summary>Timestamp scale in nanoseconds (1.000.000 means all timestamps in the Segment are expressed in milliseconds).</summary>
		public static readonly EbmlElementInfo TimecodeScale = new EbmlElementInfo(0x002AD7B1, "TimecodeScale", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 2, "Timestamp scale in nanoseconds (1.000.000 means all timestamps in the Segment are expressed in milliseconds).");

		/// <summary>Duration of the Segment (based on TimecodeScale).</summary>
		public static readonly EbmlElementInfo Duration = new EbmlElementInfo(0x00004489, "Duration", EbmlElementType.Float, EbmlElementFlags.None, 2, "Duration of the Segment (based on TimecodeScale).");

		/// <summary>Date of the origin of timestamp (value 0), i.e. production date.</summary>
		public static readonly EbmlElementInfo DateUTC = new EbmlElementInfo(0x00004461, "DateUTC", EbmlElementType.Date, EbmlElementFlags.None, 2, "Date of the origin of timestamp (value 0), i.e. production date.");

		/// <summary>General name of the Segment.</summary>
		public static readonly EbmlElementInfo Title = new EbmlElementInfo(0x00007BA9, "Title", EbmlElementType.UTF8, EbmlElementFlags.None, 2, "General name of the Segment.");

		/// <summary>Muxing application or library ("libmatroska-0.4.3").</summary>
		public static readonly EbmlElementInfo MuxingApp = new EbmlElementInfo(0x00004D80, "MuxingApp", EbmlElementType.UTF8, EbmlElementFlags.Mandatory, 2, "Muxing application or library (\"libmatroska-0.4.3\").");

		/// <summary>Writing application ("mkvmerge-0.3.3").</summary>
		public static readonly EbmlElementInfo WritingApp = new EbmlElementInfo(0x00005741, "WritingApp", EbmlElementType.UTF8, EbmlElementFlags.Mandatory, 2, "Writing application (\"mkvmerge-0.3.3\").");

		/// <summary>The Top-Level Element containing the (monolithic) Block structure.</summary>
		public static readonly EbmlElementInfo Cluster = new EbmlElementInfo(0x1F43B675, "Cluster", EbmlElementType.Master, EbmlElementFlags.None, 1, "The Top-Level Element containing the (monolithic) Block structure.");

		/// <summary>Absolute timestamp of the cluster (based on TimecodeScale).</summary>
		public static readonly EbmlElementInfo Timecode = new EbmlElementInfo(0x000000E7, "Timecode", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 2, "Absolute timestamp of the cluster (based on TimecodeScale).");

		/// <summary>The list of tracks that are not used in that part of the stream. It is useful when using overlay tracks on seeking. Then you should decide what track to use.</summary>
		public static readonly EbmlElementInfo SilentTracks = new EbmlElementInfo(0x00005854, "SilentTracks", EbmlElementType.Master, EbmlElementFlags.None, 2, "The list of tracks that are not used in that part of the stream. It is useful when using overlay tracks on seeking. Then you should decide what track to use.");

		/// <summary>One of the track number that are not used from now on in the stream. It could change later if not specified as silent in a further Cluster.</summary>
		public static readonly EbmlElementInfo SilentTrackNumber = new EbmlElementInfo(0x000058D7, "SilentTrackNumber", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "One of the track number that are not used from now on in the stream. It could change later if not specified as silent in a further Cluster.");

		/// <summary>The Position of the Cluster in the Segment (0 in live broadcast streams). It might help to resynchronise offset on damaged streams.</summary>
		public static readonly EbmlElementInfo Position = new EbmlElementInfo(0x000000A7, "Position", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 2, "The Position of the Cluster in the Segment (0 in live broadcast streams). It might help to resynchronise offset on damaged streams.");

		/// <summary>Size of the previous Cluster, in octets. Can be useful for backward playing.</summary>
		public static readonly EbmlElementInfo PrevSize = new EbmlElementInfo(0x000000AB, "PrevSize", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 2, "Size of the previous Cluster, in octets. Can be useful for backward playing.");

		/// <summary>Similar to Block but without all the extra information, mostly used to reduced overhead when no extra feature is needed. (see SimpleBlock Structure)</summary>
		public static readonly EbmlElementInfo SimpleBlock = new EbmlElementInfo(0x000000A3, "SimpleBlock", EbmlElementType.Binary, EbmlElementFlags.None, 2, "Similar to Block but without all the extra information, mostly used to reduced overhead when no extra feature is needed. (see SimpleBlock Structure)");

		/// <summary>Basic container of information containing a single Block or BlockVirtual, and information specific to that Block/VirtualBlock.</summary>
		public static readonly EbmlElementInfo BlockGroup = new EbmlElementInfo(0x000000A0, "BlockGroup", EbmlElementType.Master, EbmlElementFlags.None, 2, "Basic container of information containing a single Block or BlockVirtual, and information specific to that Block/VirtualBlock.");

		/// <summary>Block containing the actual data to be rendered and a timestamp relative to the Cluster Timecode. (see Block Structure)</summary>
		public static readonly EbmlElementInfo Block = new EbmlElementInfo(0x000000A1, "Block", EbmlElementType.Binary, EbmlElementFlags.Mandatory, 3, "Block containing the actual data to be rendered and a timestamp relative to the Cluster Timecode. (see Block Structure)");

		/// <summary>A Block with no data. It must be stored in the stream at the place the real Block should be in display order. (see Block Virtual)</summary>
		public static readonly EbmlElementInfo BlockVirtual = new EbmlElementInfo(0x000000A2, "BlockVirtual", EbmlElementType.Binary, EbmlElementFlags.None, 3, "A Block with no data. It must be stored in the stream at the place the real Block should be in display order. (see Block Virtual)");

		/// <summary>Contain additional blocks to complete the main one. An EBML parser that has no knowledge of the Block structure could still see and use/skip these data.</summary>
		public static readonly EbmlElementInfo BlockAdditions = new EbmlElementInfo(0x000075A1, "BlockAdditions", EbmlElementType.Master, EbmlElementFlags.None, 3, "Contain additional blocks to complete the main one. An EBML parser that has no knowledge of the Block structure could still see and use/skip these data.");

		/// <summary>Contain the BlockAdditional and some parameters.</summary>
		public static readonly EbmlElementInfo BlockMore = new EbmlElementInfo(0x000000A6, "BlockMore", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 4, "Contain the BlockAdditional and some parameters.");

		/// <summary>An ID to identify the BlockAdditional level.</summary>
		public static readonly EbmlElementInfo BlockAddID = new EbmlElementInfo(0x000000EE, "BlockAddID", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 5, "An ID to identify the BlockAdditional level.");

		/// <summary>Interpreted by the codec as it wishes (using the BlockAddID).</summary>
		public static readonly EbmlElementInfo BlockAdditional = new EbmlElementInfo(0x000000A5, "BlockAdditional", EbmlElementType.Binary, EbmlElementFlags.Mandatory, 5, "Interpreted by the codec as it wishes (using the BlockAddID).");

		/// <summary>The duration of the Block (based on TimecodeScale). This Element is mandatory when DefaultDuration is set for the track (but can be omitted as other default values). When not written and with no DefaultDuration, the value is assumed to be the difference between the timestamp of this Block and the timestamp of the next Block in "display" order (not coding order). This Element can be useful at the end of a Track (as there is not other Block available), or when there is a break in a track like for subtitle tracks. When set to 0 that means the frame is not a keyframe.</summary>
		public static readonly EbmlElementInfo BlockDuration = new EbmlElementInfo(0x0000009B, "BlockDuration", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "The duration of the Block (based on TimecodeScale). This Element is mandatory when DefaultDuration is set for the track (but can be omitted as other default values). When not written and with no DefaultDuration, the value is assumed to be the difference between the timestamp of this Block and the timestamp of the next Block in \"display\" order (not coding order). This Element can be useful at the end of a Track (as there is not other Block available), or when there is a break in a track like for subtitle tracks. When set to 0 that means the frame is not a keyframe.");

		/// <summary>This frame is referenced and has the specified cache priority. In cache only a frame of the same or higher priority can replace this frame. A value of 0 means the frame is not referenced.</summary>
		public static readonly EbmlElementInfo ReferencePriority = new EbmlElementInfo(0x000000FA, "ReferencePriority", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "This frame is referenced and has the specified cache priority. In cache only a frame of the same or higher priority can replace this frame. A value of 0 means the frame is not referenced.");

		/// <summary>Timestamp of another frame used as a reference (ie: B or P frame). The timestamp is relative to the block it's attached to.</summary>
		public static readonly EbmlElementInfo ReferenceBlock = new EbmlElementInfo(0x000000FB, "ReferenceBlock", EbmlElementType.SignedInteger, EbmlElementFlags.None, 3, "Timestamp of another frame used as a reference (ie: B or P frame). The timestamp is relative to the block it's attached to.");

		/// <summary>Relative position of the data that should be in position of the virtual block.</summary>
		public static readonly EbmlElementInfo ReferenceVirtual = new EbmlElementInfo(0x000000FD, "ReferenceVirtual", EbmlElementType.SignedInteger, EbmlElementFlags.None, 3, "Relative position of the data that should be in position of the virtual block.");

		/// <summary>The new codec state to use. Data interpretation is private to the codec. This information should always be referenced by a seek entry.</summary>
		public static readonly EbmlElementInfo CodecState = new EbmlElementInfo(0x000000A4, "CodecState", EbmlElementType.Binary, EbmlElementFlags.None, 3, "The new codec state to use. Data interpretation is private to the codec. This information should always be referenced by a seek entry.");

		/// <summary>Duration in nanoseconds of the silent data added to the Block (padding at the end of the Block for positive value, at the beginning of the Block for negative value). The duration of DiscardPadding is not calculated in the duration of the TrackEntry and should be discarded during playback.</summary>
		public static readonly EbmlElementInfo DiscardPadding = new EbmlElementInfo(0x000075A2, "DiscardPadding", EbmlElementType.SignedInteger, EbmlElementFlags.None, 3, "Duration in nanoseconds of the silent data added to the Block (padding at the end of the Block for positive value, at the beginning of the Block for negative value). The duration of DiscardPadding is not calculated in the duration of the TrackEntry and should be discarded during playback.");

		/// <summary>Contains slices description.</summary>
		public static readonly EbmlElementInfo Slices = new EbmlElementInfo(0x0000008E, "Slices", EbmlElementType.Master, EbmlElementFlags.None, 3, "Contains slices description.");

		/// <summary>Contains extra time information about the data contained in the Block. While there are a few files in the wild with this Element, it is no longer in use and has been deprecated. Being able to interpret this Element is not required for playback.</summary>
		public static readonly EbmlElementInfo TimeSlice = new EbmlElementInfo(0x000000E8, "TimeSlice", EbmlElementType.Master, EbmlElementFlags.None, 4, "Contains extra time information about the data contained in the Block. While there are a few files in the wild with this Element, it is no longer in use and has been deprecated. Being able to interpret this Element is not required for playback.");

		/// <summary>The reverse number of the frame in the lace (0 is the last frame, 1 is the next to last, etc). While there are a few files in the wild with this Element, it is no longer in use and has been deprecated. Being able to interpret this Element is not required for playback.</summary>
		public static readonly EbmlElementInfo LaceNumber = new EbmlElementInfo(0x000000CC, "LaceNumber", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The reverse number of the frame in the lace (0 is the last frame, 1 is the next to last, etc). While there are a few files in the wild with this Element, it is no longer in use and has been deprecated. Being able to interpret this Element is not required for playback.");

		/// <summary>The number of the frame to generate from this lace with this delay (allow you to generate many frames from the same Block/Frame).</summary>
		public static readonly EbmlElementInfo FrameNumber = new EbmlElementInfo(0x000000CD, "FrameNumber", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The number of the frame to generate from this lace with this delay (allow you to generate many frames from the same Block/Frame).");

		/// <summary>The ID of the BlockAdditional Element (0 is the main Block).</summary>
		public static readonly EbmlElementInfo BlockAdditionID = new EbmlElementInfo(0x000000CB, "BlockAdditionID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The ID of the BlockAdditional Element (0 is the main Block).");

		/// <summary>The (scaled) delay to apply to the Element.</summary>
		public static readonly EbmlElementInfo Delay = new EbmlElementInfo(0x000000CE, "Delay", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The (scaled) delay to apply to the Element.");

		/// <summary>The (scaled) duration to apply to the Element.</summary>
		public static readonly EbmlElementInfo SliceDuration = new EbmlElementInfo(0x000000CF, "SliceDuration", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The (scaled) duration to apply to the Element.");

		/// <summary>DivX trick track extenstions</summary>
		public static readonly EbmlElementInfo ReferenceFrame = new EbmlElementInfo(0x000000C8, "ReferenceFrame", EbmlElementType.Master, EbmlElementFlags.None, 3, "DivX trick track extenstions");

		/// <summary>DivX trick track extenstions</summary>
		public static readonly EbmlElementInfo ReferenceOffset = new EbmlElementInfo(0x000000C9, "ReferenceOffset", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "DivX trick track extenstions");

		/// <summary>DivX trick track extenstions</summary>
		public static readonly EbmlElementInfo ReferenceTimeCode = new EbmlElementInfo(0x000000CA, "ReferenceTimeCode", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "DivX trick track extenstions");

		/// <summary>Similar to SimpleBlock but the data inside the Block are Transformed (encrypt and/or signed). (see EncryptedBlock Structure)</summary>
		public static readonly EbmlElementInfo EncryptedBlock = new EbmlElementInfo(0x000000AF, "EncryptedBlock", EbmlElementType.Binary, EbmlElementFlags.None, 2, "Similar to SimpleBlock but the data inside the Block are Transformed (encrypt and/or signed). (see EncryptedBlock Structure)");

		/// <summary>A Top-Level Element of information with many tracks described.</summary>
		public static readonly EbmlElementInfo Tracks = new EbmlElementInfo(0x1654AE6B, "Tracks", EbmlElementType.Master, EbmlElementFlags.None, 1, "A Top-Level Element of information with many tracks described.");

		/// <summary>Describes a track with all Elements.</summary>
		public static readonly EbmlElementInfo TrackEntry = new EbmlElementInfo(0x000000AE, "TrackEntry", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 2, "Describes a track with all Elements.");

		/// <summary>The track number as used in the Block Header (using more than 127 tracks is not encouraged, though the design allows an unlimited number).</summary>
		public static readonly EbmlElementInfo TrackNumber = new EbmlElementInfo(0x000000D7, "TrackNumber", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "The track number as used in the Block Header (using more than 127 tracks is not encouraged, though the design allows an unlimited number).");

		/// <summary>A unique ID to identify the Track. This should be kept the same when making a direct stream copy of the Track to another file.</summary>
		public static readonly EbmlElementInfo TrackUID = new EbmlElementInfo(0x000073C5, "TrackUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "A unique ID to identify the Track. This should be kept the same when making a direct stream copy of the Track to another file.");

		/// <summary>A set of track types coded on 8 bits (1: video, 2: audio, 3: complex, 0x10: logo, 0x11: subtitle, 0x12: buttons, 0x20: control).</summary>
		public static readonly EbmlElementInfo TrackType = new EbmlElementInfo(0x00000083, "TrackType", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "A set of track types coded on 8 bits (1: video, 2: audio, 3: complex, 0x10: logo, 0x11: subtitle, 0x12: buttons, 0x20: control).");

		/// <summary>Set if the track is usable. (1 bit)</summary>
		public static readonly EbmlElementInfo FlagEnabled = new EbmlElementInfo(0x000000B9, "FlagEnabled", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "Set if the track is usable. (1 bit)");

		/// <summary>Set if that track (audio, video or subs) SHOULD be active if no language found matches the user preference. (1 bit)</summary>
		public static readonly EbmlElementInfo FlagDefault = new EbmlElementInfo(0x00000088, "FlagDefault", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "Set if that track (audio, video or subs) SHOULD be active if no language found matches the user preference. (1 bit)");

		/// <summary>Set if that track MUST be active during playback. There can be many forced track for a kind (audio, video or subs), the player should select the one which language matches the user preference or the default + forced track. Overlay MAY happen between a forced and non-forced track of the same kind. (1 bit)</summary>
		public static readonly EbmlElementInfo FlagForced = new EbmlElementInfo(0x000055AA, "FlagForced", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "Set if that track MUST be active during playback. There can be many forced track for a kind (audio, video or subs), the player should select the one which language matches the user preference or the default + forced track. Overlay MAY happen between a forced and non-forced track of the same kind. (1 bit)");

		/// <summary>Set if the track may contain blocks using lacing. (1 bit)</summary>
		public static readonly EbmlElementInfo FlagLacing = new EbmlElementInfo(0x0000009C, "FlagLacing", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "Set if the track may contain blocks using lacing. (1 bit)");

		/// <summary>The minimum number of frames a player should be able to cache during playback. If set to 0, the reference pseudo-cache system is not used.</summary>
		public static readonly EbmlElementInfo MinCache = new EbmlElementInfo(0x00006DE7, "MinCache", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "The minimum number of frames a player should be able to cache during playback. If set to 0, the reference pseudo-cache system is not used.");

		/// <summary>The maximum cache size required to store referenced frames in and the current frame. 0 means no cache is needed.</summary>
		public static readonly EbmlElementInfo MaxCache = new EbmlElementInfo(0x00006DF8, "MaxCache", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "The maximum cache size required to store referenced frames in and the current frame. 0 means no cache is needed.");

		/// <summary>Number of nanoseconds (not scaled via TimecodeScale) per frame ('frame' in the Matroska sense -- one Element put into a (Simple)Block).</summary>
		public static readonly EbmlElementInfo DefaultDuration = new EbmlElementInfo(0x0023E383, "DefaultDuration", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "Number of nanoseconds (not scaled via TimecodeScale) per frame ('frame' in the Matroska sense -- one Element put into a (Simple)Block).");

		/// <summary>The period in nanoseconds (not scaled by TimcodeScale) between two successive fields at the output of the decoding process (see the notes)</summary>
		public static readonly EbmlElementInfo DefaultDecodedFieldDuration = new EbmlElementInfo(0x00234E7A, "DefaultDecodedFieldDuration", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "The period in nanoseconds (not scaled by TimcodeScale) between two successive fields at the output of the decoding process (see the notes)");

		/// <summary>DEPRECATED, DO NOT USE. The scale to apply on this track to work at normal speed in relation with other tracks (mostly used to adjust video speed when the audio length differs).</summary>
		public static readonly EbmlElementInfo TrackTimecodeScale = new EbmlElementInfo(0x0023314F, "TrackTimecodeScale", EbmlElementType.Float, EbmlElementFlags.Mandatory, 3, "DEPRECATED, DO NOT USE. The scale to apply on this track to work at normal speed in relation with other tracks (mostly used to adjust video speed when the audio length differs).");

		/// <summary>A value to add to the Block's Timestamp. This can be used to adjust the playback offset of a track.</summary>
		public static readonly EbmlElementInfo TrackOffset = new EbmlElementInfo(0x0000537F, "TrackOffset", EbmlElementType.SignedInteger, EbmlElementFlags.None, 3, "A value to add to the Block's Timestamp. This can be used to adjust the playback offset of a track.");

		/// <summary>The maximum value of BlockAddID. A value 0 means there is no BlockAdditions for this track.</summary>
		public static readonly EbmlElementInfo MaxBlockAdditionID = new EbmlElementInfo(0x000055EE, "MaxBlockAdditionID", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "The maximum value of BlockAddID. A value 0 means there is no BlockAdditions for this track.");

		/// <summary>A human-readable track name.</summary>
		public static readonly EbmlElementInfo Name = new EbmlElementInfo(0x0000536E, "Name", EbmlElementType.UTF8, EbmlElementFlags.None, 3, "A human-readable track name.");

		/// <summary>Specifies the language of the track in the Matroska languages form.</summary>
		public static readonly EbmlElementInfo Language = new EbmlElementInfo(0x0022B59C, "Language", EbmlElementType.String, EbmlElementFlags.None, 3, "Specifies the language of the track in the Matroska languages form.");

		/// <summary>An ID corresponding to the codec, see the codec page for more info.</summary>
		public static readonly EbmlElementInfo CodecID = new EbmlElementInfo(0x00000086, "CodecID", EbmlElementType.String, EbmlElementFlags.Mandatory, 3, "An ID corresponding to the codec, see the codec page for more info.");

		/// <summary>Private data only known to the codec.</summary>
		public static readonly EbmlElementInfo CodecPrivate = new EbmlElementInfo(0x000063A2, "CodecPrivate", EbmlElementType.Binary, EbmlElementFlags.None, 3, "Private data only known to the codec.");

		/// <summary>A human-readable string specifying the codec.</summary>
		public static readonly EbmlElementInfo CodecName = new EbmlElementInfo(0x00258688, "CodecName", EbmlElementType.UTF8, EbmlElementFlags.None, 3, "A human-readable string specifying the codec.");

		/// <summary>The UID of an attachment that is used by this codec.</summary>
		public static readonly EbmlElementInfo AttachmentLink = new EbmlElementInfo(0x00007446, "AttachmentLink", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "The UID of an attachment that is used by this codec.");

		/// <summary>A string describing the encoding setting used.</summary>
		public static readonly EbmlElementInfo CodecSettings = new EbmlElementInfo(0x003A9697, "CodecSettings", EbmlElementType.UTF8, EbmlElementFlags.None, 3, "A string describing the encoding setting used.");

		/// <summary>A URL to find information about the codec used.</summary>
		public static readonly EbmlElementInfo CodecInfoURL = new EbmlElementInfo(0x003B4040, "CodecInfoURL", EbmlElementType.String, EbmlElementFlags.None, 3, "A URL to find information about the codec used.");

		/// <summary>A URL to download about the codec used.</summary>
		public static readonly EbmlElementInfo CodecDownloadURL = new EbmlElementInfo(0x0026B240, "CodecDownloadURL", EbmlElementType.String, EbmlElementFlags.None, 3, "A URL to download about the codec used.");

		/// <summary>The codec can decode potentially damaged data (1 bit).</summary>
		public static readonly EbmlElementInfo CodecDecodeAll = new EbmlElementInfo(0x000000AA, "CodecDecodeAll", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "The codec can decode potentially damaged data (1 bit).");

		/// <summary>Specify that this track is an overlay track for the Track specified (in the u-integer). That means when this track has a gap (see SilentTracks) the overlay track should be used instead. The order of multiple TrackOverlay matters, the first one is the one that should be used. If not found it should be the second, etc.</summary>
		public static readonly EbmlElementInfo TrackOverlay = new EbmlElementInfo(0x00006FAB, "TrackOverlay", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "Specify that this track is an overlay track for the Track specified (in the u-integer). That means when this track has a gap (see SilentTracks) the overlay track should be used instead. The order of multiple TrackOverlay matters, the first one is the one that should be used. If not found it should be the second, etc.");

		/// <summary>CodecDelay is The codec-built-in delay in nanoseconds. This value must be subtracted from each block timestamp in order to get the actual timestamp. The value should be small so the muxing of tracks with the same actual timestamp are in the same Cluster.</summary>
		public static readonly EbmlElementInfo CodecDelay = new EbmlElementInfo(0x000056AA, "CodecDelay", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "CodecDelay is The codec-built-in delay in nanoseconds. This value must be subtracted from each block timestamp in order to get the actual timestamp. The value should be small so the muxing of tracks with the same actual timestamp are in the same Cluster.");

		/// <summary>After a discontinuity, SeekPreRoll is the duration in nanoseconds of the data the decoder must decode before the decoded data is valid.</summary>
		public static readonly EbmlElementInfo SeekPreRoll = new EbmlElementInfo(0x000056BB, "SeekPreRoll", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "After a discontinuity, SeekPreRoll is the duration in nanoseconds of the data the decoder must decode before the decoded data is valid.");

		/// <summary>The track identification for the given Chapter Codec.</summary>
		public static readonly EbmlElementInfo TrackTranslate = new EbmlElementInfo(0x00006624, "TrackTranslate", EbmlElementType.Master, EbmlElementFlags.None, 3, "The track identification for the given Chapter Codec.");

		/// <summary>Specify an edition UID on which this translation applies. When not specified, it means for all editions found in the Segment.</summary>
		public static readonly EbmlElementInfo TrackTranslateEditionUID = new EbmlElementInfo(0x000066FC, "TrackTranslateEditionUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Specify an edition UID on which this translation applies. When not specified, it means for all editions found in the Segment.");

		/// <summary>The chapter codec using this ID (0: Matroska Script, 1: DVD-menu).</summary>
		public static readonly EbmlElementInfo TrackTranslateCodec = new EbmlElementInfo(0x000066BF, "TrackTranslateCodec", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "The chapter codec using this ID (0: Matroska Script, 1: DVD-menu).");

		/// <summary>The binary value used to represent this track in the chapter codec data. The format depends on the ChapProcessCodecID used.</summary>
		public static readonly EbmlElementInfo TrackTranslateTrackID = new EbmlElementInfo(0x000066A5, "TrackTranslateTrackID", EbmlElementType.Binary, EbmlElementFlags.Mandatory, 4, "The binary value used to represent this track in the chapter codec data. The format depends on the ChapProcessCodecID used.");

		/// <summary>Video settings.</summary>
		public static readonly EbmlElementInfo Video = new EbmlElementInfo(0x000000E0, "Video", EbmlElementType.Master, EbmlElementFlags.None, 3, "Video settings.");

		/// <summary>A flag to declare is the video is known to be progressive or interlaced and if applicable to declare details about the interlacement. (0: undetermined, 1: interlaced, 2: progressive)</summary>
		public static readonly EbmlElementInfo FlagInterlaced = new EbmlElementInfo(0x0000009A, "FlagInterlaced", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "A flag to declare is the video is known to be progressive or interlaced and if applicable to declare details about the interlacement. (0: undetermined, 1: interlaced, 2: progressive)");

		/// <summary>Declare the field ordering of the video. If FlagInterlaced is not set to 1, this Element MUST be ignored. (0: Progressive, 1: Interlaced with top field display first and top field stored first, 2: Undetermined field order, 6: Interlaced with bottom field displayed first and bottom field stored first, 9: Interlaced with bottom field displayed first and top field stored first, 14: Interlaced with top field displayed first and bottom field stored first)</summary>
		public static readonly EbmlElementInfo FieldOrder = new EbmlElementInfo(0x0000009D, "FieldOrder", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "Declare the field ordering of the video. If FlagInterlaced is not set to 1, this Element MUST be ignored. (0: Progressive, 1: Interlaced with top field display first and top field stored first, 2: Undetermined field order, 6: Interlaced with bottom field displayed first and bottom field stored first, 9: Interlaced with bottom field displayed first and top field stored first, 14: Interlaced with top field displayed first and bottom field stored first)");

		/// <summary>Stereo-3D video mode (0: mono, 1: side by side (left eye is first), 2: top-bottom (right eye is first), 3: top-bottom (left eye is first), 4: checkboard (right is first), 5: checkboard (left is first), 6: row interleaved (right is first), 7: row interleaved (left is first), 8: column interleaved (right is first), 9: column interleaved (left is first), 10: anaglyph (cyan/red), 11: side by side (right eye is first), 12: anaglyph (green/magenta), 13 both eyes laced in one Block (left eye is first), 14 both eyes laced in one Block (right eye is first)) . There are some more details on 3D support in the Specification Notes.</summary>
		public static readonly EbmlElementInfo StereoMode = new EbmlElementInfo(0x000053B8, "StereoMode", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Stereo-3D video mode (0: mono, 1: side by side (left eye is first), 2: top-bottom (right eye is first), 3: top-bottom (left eye is first), 4: checkboard (right is first), 5: checkboard (left is first), 6: row interleaved (right is first), 7: row interleaved (left is first), 8: column interleaved (right is first), 9: column interleaved (left is first), 10: anaglyph (cyan/red), 11: side by side (right eye is first), 12: anaglyph (green/magenta), 13 both eyes laced in one Block (left eye is first), 14 both eyes laced in one Block (right eye is first)) . There are some more details on 3D support in the Specification Notes.");

		/// <summary>Alpha Video Mode. Presence of this Element indicates that the BlockAdditional Element could contain Alpha data.</summary>
		public static readonly EbmlElementInfo AlphaMode = new EbmlElementInfo(0x000053C0, "AlphaMode", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Alpha Video Mode. Presence of this Element indicates that the BlockAdditional Element could contain Alpha data.");

		/// <summary>DEPRECATED, DO NOT USE. Bogus StereoMode value used in old versions of libmatroska. (0: mono, 1: right eye, 2: left eye, 3: both eyes).</summary>
		public static readonly EbmlElementInfo OldStereoMode = new EbmlElementInfo(0x000053B9, "OldStereoMode", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "DEPRECATED, DO NOT USE. Bogus StereoMode value used in old versions of libmatroska. (0: mono, 1: right eye, 2: left eye, 3: both eyes).");

		/// <summary>Width of the encoded video frames in pixels.</summary>
		public static readonly EbmlElementInfo PixelWidth = new EbmlElementInfo(0x000000B0, "PixelWidth", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "Width of the encoded video frames in pixels.");

		/// <summary>Height of the encoded video frames in pixels.</summary>
		public static readonly EbmlElementInfo PixelHeight = new EbmlElementInfo(0x000000BA, "PixelHeight", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "Height of the encoded video frames in pixels.");

		/// <summary>The number of video pixels to remove at the bottom of the image (for HDTV content).</summary>
		public static readonly EbmlElementInfo PixelCropBottom = new EbmlElementInfo(0x000054AA, "PixelCropBottom", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "The number of video pixels to remove at the bottom of the image (for HDTV content).");

		/// <summary>The number of video pixels to remove at the top of the image.</summary>
		public static readonly EbmlElementInfo PixelCropTop = new EbmlElementInfo(0x000054BB, "PixelCropTop", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "The number of video pixels to remove at the top of the image.");

		/// <summary>The number of video pixels to remove on the left of the image.</summary>
		public static readonly EbmlElementInfo PixelCropLeft = new EbmlElementInfo(0x000054CC, "PixelCropLeft", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "The number of video pixels to remove on the left of the image.");

		/// <summary>The number of video pixels to remove on the right of the image.</summary>
		public static readonly EbmlElementInfo PixelCropRight = new EbmlElementInfo(0x000054DD, "PixelCropRight", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "The number of video pixels to remove on the right of the image.");

		/// <summary>Width of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements). The default value is only valid when DisplayUnit is 0.</summary>
		public static readonly EbmlElementInfo DisplayWidth = new EbmlElementInfo(0x000054B0, "DisplayWidth", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Width of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements). The default value is only valid when DisplayUnit is 0.");

		/// <summary>Height of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements). The default value is only valid when DisplayUnit is 0.</summary>
		public static readonly EbmlElementInfo DisplayHeight = new EbmlElementInfo(0x000054BA, "DisplayHeight", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Height of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements). The default value is only valid when DisplayUnit is 0.");

		/// <summary>How DisplayWidth & DisplayHeight should be interpreted (0: pixels, 1: centimeters, 2: inches, 3: Display Aspect Ratio, 4: Unknown).</summary>
		public static readonly EbmlElementInfo DisplayUnit = new EbmlElementInfo(0x000054B2, "DisplayUnit", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "How DisplayWidth & DisplayHeight should be interpreted (0: pixels, 1: centimeters, 2: inches, 3: Display Aspect Ratio, 4: Unknown).");

		/// <summary>Specify the possible modifications to the aspect ratio (0: free resizing, 1: keep aspect ratio, 2: fixed).</summary>
		public static readonly EbmlElementInfo AspectRatioType = new EbmlElementInfo(0x000054B3, "AspectRatioType", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Specify the possible modifications to the aspect ratio (0: free resizing, 1: keep aspect ratio, 2: fixed).");

		/// <summary>Same value as in AVI (32 bits).</summary>
		public static readonly EbmlElementInfo ColourSpace = new EbmlElementInfo(0x002EB524, "ColourSpace", EbmlElementType.Binary, EbmlElementFlags.None, 4, "Same value as in AVI (32 bits).");

		/// <summary>Gamma Value.</summary>
		public static readonly EbmlElementInfo GammaValue = new EbmlElementInfo(0x002FB523, "GammaValue", EbmlElementType.Float, EbmlElementFlags.None, 4, "Gamma Value.");

		/// <summary>Number of frames per second. Informational only.</summary>
		public static readonly EbmlElementInfo FrameRate = new EbmlElementInfo(0x002383E3, "FrameRate", EbmlElementType.Float, EbmlElementFlags.None, 4, "Number of frames per second. Informational only.");

		/// <summary> Settings describing the colour format.</summary>
		public static readonly EbmlElementInfo Colour = new EbmlElementInfo(0x000055B0, "Colour", EbmlElementType.Master, EbmlElementFlags.None, 4, " Settings describing the colour format.");

		/// <summary>The Matrix Coefficients of the video used to derive luma and chroma values from reg, green, and blue color primaries. For clarity, the value and meanings for MatrixCoefficients are adopted from Table 4 of ISO/IEC 23001-8:2013/DCOR1. (0:GBR, 1: BT709, 2: Unspecified, 3: Reserved, 4: FCC, 5: BT470BG, 6: SMPTE 170M, 7: SMPTE 240M, 8: YCOCG, 9: BT2020 Non-constant Luminance, 10: BT2020 Constant Luminance)</summary>
		public static readonly EbmlElementInfo MatrixCoefficients = new EbmlElementInfo(0x000055B1, "MatrixCoefficients", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The Matrix Coefficients of the video used to derive luma and chroma values from reg, green, and blue color primaries. For clarity, the value and meanings for MatrixCoefficients are adopted from Table 4 of ISO/IEC 23001-8:2013/DCOR1. (0:GBR, 1: BT709, 2: Unspecified, 3: Reserved, 4: FCC, 5: BT470BG, 6: SMPTE 170M, 7: SMPTE 240M, 8: YCOCG, 9: BT2020 Non-constant Luminance, 10: BT2020 Constant Luminance)");

		/// <summary>Number of decoded bits per channel. A value of 0 indicates that the BitsPerChannel is unspecified.</summary>
		public static readonly EbmlElementInfo BitsPerChannel = new EbmlElementInfo(0x000055B2, "BitsPerChannel", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "Number of decoded bits per channel. A value of 0 indicates that the BitsPerChannel is unspecified.");

		/// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed horizontally. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingHorz should be set to 1.</summary>
		public static readonly EbmlElementInfo ChromaSubsamplingHorz = new EbmlElementInfo(0x000055B3, "ChromaSubsamplingHorz", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The amount of pixels to remove in the Cr and Cb channels for every pixel not removed horizontally. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingHorz should be set to 1.");

		/// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed vertically. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingVert should be set to 1.</summary>
		public static readonly EbmlElementInfo ChromaSubsamplingVert = new EbmlElementInfo(0x000055B4, "ChromaSubsamplingVert", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The amount of pixels to remove in the Cr and Cb channels for every pixel not removed vertically. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingVert should be set to 1.");

		/// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed horizontally. This is additive with ChromaSubsamplingHorz. Example: For video with 4:2:1 chroma subsampling, the ChromaSubsamplingHorz should be set to 1 and CbSubsamplingHorz should be set to 1.</summary>
		public static readonly EbmlElementInfo CbSubsamplingHorz = new EbmlElementInfo(0x000055B5, "CbSubsamplingHorz", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The amount of pixels to remove in the Cb channel for every pixel not removed horizontally. This is additive with ChromaSubsamplingHorz. Example: For video with 4:2:1 chroma subsampling, the ChromaSubsamplingHorz should be set to 1 and CbSubsamplingHorz should be set to 1.");

		/// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed vertically. This is additive with ChromaSubsamplingVert.</summary>
		public static readonly EbmlElementInfo CbSubsamplingVert = new EbmlElementInfo(0x000055B6, "CbSubsamplingVert", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The amount of pixels to remove in the Cb channel for every pixel not removed vertically. This is additive with ChromaSubsamplingVert.");

		/// <summary>How chroma is subsampled horizontally. (0: Unspecified, 1: Left Collocated, 2: Half)</summary>
		public static readonly EbmlElementInfo ChromaSitingHorz = new EbmlElementInfo(0x000055B7, "ChromaSitingHorz", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "How chroma is subsampled horizontally. (0: Unspecified, 1: Left Collocated, 2: Half)");

		/// <summary>How chroma is subsampled vertically. (0: Unspecified, 1: Top Collocated, 2: Half)</summary>
		public static readonly EbmlElementInfo ChromaSitingVert = new EbmlElementInfo(0x000055B8, "ChromaSitingVert", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "How chroma is subsampled vertically. (0: Unspecified, 1: Top Collocated, 2: Half)");

		/// <summary>Clipping of the color ranges. (0: Unspecified, 1: Broadcast Range, 2: Full range (no clipping), 3: Defined by MatrixCoefficients/TransferCharacteristics)</summary>
		public static readonly EbmlElementInfo Range = new EbmlElementInfo(0x000055B9, "Range", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "Clipping of the color ranges. (0: Unspecified, 1: Broadcast Range, 2: Full range (no clipping), 3: Defined by MatrixCoefficients/TransferCharacteristics)");

		/// <summary>The transfer characteristics of the video. For clarity, the value and meanings for TransferCharacteristics 1-15 are adopted from Table 3 of ISO/IEC 23001-8:2013/DCOR1. TransferCharacteristics 16-18 are proposed values. (0: Reserved, 1: ITU-R BT.709, 2: Unspecified, 3: Reserved, 4: Gamma 2.2 curve, 5: Gamma 2.8 curve, 6: SMPTE 170M, 7: SMPTE 240M, 8: Linear, 9: Log, 10: Log Sqrt, 11: IEC 61966-2-4, 12: ITU-R BT.1361 Extended Colour Gamut, 13: IEC 61966-2-1, 14: ITU-R BT.2020 10 bit, 15: ITU-R BT.2020 12 bit, 16: SMPTE ST 2084, 17: SMPTE ST 428-1 18: ARIB STD-B67 (HLG))</summary>
		public static readonly EbmlElementInfo TransferCharacteristics = new EbmlElementInfo(0x000055BA, "TransferCharacteristics", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The transfer characteristics of the video. For clarity, the value and meanings for TransferCharacteristics 1-15 are adopted from Table 3 of ISO/IEC 23001-8:2013/DCOR1. TransferCharacteristics 16-18 are proposed values. (0: Reserved, 1: ITU-R BT.709, 2: Unspecified, 3: Reserved, 4: Gamma 2.2 curve, 5: Gamma 2.8 curve, 6: SMPTE 170M, 7: SMPTE 240M, 8: Linear, 9: Log, 10: Log Sqrt, 11: IEC 61966-2-4, 12: ITU-R BT.1361 Extended Colour Gamut, 13: IEC 61966-2-1, 14: ITU-R BT.2020 10 bit, 15: ITU-R BT.2020 12 bit, 16: SMPTE ST 2084, 17: SMPTE ST 428-1 18: ARIB STD-B67 (HLG))");

		/// <summary>The colour primaries of the video. For clarity, the value and meanings for Primaries are adopted from Table 2 of ISO/IEC 23001-8:2013/DCOR1. (0: Reserved, 1: ITU-R BT.709, 2: Unspecified, 3: Reserved, 4: ITU-R BT.470M, 5: ITU-R BT.470BG, 6: SMPTE 170M, 7: SMPTE 240M, 8: FILM, 9: ITU-R BT.2020, 10: SMPTE ST 428-1, 22: JEDEC P22 phosphors)</summary>
		public static readonly EbmlElementInfo Primaries = new EbmlElementInfo(0x000055BB, "Primaries", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The colour primaries of the video. For clarity, the value and meanings for Primaries are adopted from Table 2 of ISO/IEC 23001-8:2013/DCOR1. (0: Reserved, 1: ITU-R BT.709, 2: Unspecified, 3: Reserved, 4: ITU-R BT.470M, 5: ITU-R BT.470BG, 6: SMPTE 170M, 7: SMPTE 240M, 8: FILM, 9: ITU-R BT.2020, 10: SMPTE ST 428-1, 22: JEDEC P22 phosphors)");

		/// <summary>Maximum brightness of a single pixel (Maximum Content Light Level) in candelas per square meter (cd/mý).</summary>
		public static readonly EbmlElementInfo MaxCLL = new EbmlElementInfo(0x000055BC, "MaxCLL", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "Maximum brightness of a single pixel (Maximum Content Light Level) in candelas per square meter (cd/mý).");

		/// <summary>Maximum brightness of a single full frame (Maximum Frame-Average Light Level) in candelas per square meter (cd/mý).</summary>
		public static readonly EbmlElementInfo MaxFALL = new EbmlElementInfo(0x000055BD, "MaxFALL", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "Maximum brightness of a single full frame (Maximum Frame-Average Light Level) in candelas per square meter (cd/mý).");

		/// <summary>SMPTE 2086 mastering data.</summary>
		public static readonly EbmlElementInfo MasteringMetadata = new EbmlElementInfo(0x000055D0, "MasteringMetadata", EbmlElementType.Master, EbmlElementFlags.None, 5, "SMPTE 2086 mastering data.");

		/// <summary>Red X chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly EbmlElementInfo PrimaryRChromaticityX = new EbmlElementInfo(0x000055D1, "PrimaryRChromaticityX", EbmlElementType.Float, EbmlElementFlags.None, 6, "Red X chromaticity coordinate as defined by CIE 1931.");

		/// <summary>Red Y chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly EbmlElementInfo PrimaryRChromaticityY = new EbmlElementInfo(0x000055D2, "PrimaryRChromaticityY", EbmlElementType.Float, EbmlElementFlags.None, 6, "Red Y chromaticity coordinate as defined by CIE 1931.");

		/// <summary>Green X chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly EbmlElementInfo PrimaryGChromaticityX = new EbmlElementInfo(0x000055D3, "PrimaryGChromaticityX", EbmlElementType.Float, EbmlElementFlags.None, 6, "Green X chromaticity coordinate as defined by CIE 1931.");

		/// <summary>Green Y chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly EbmlElementInfo PrimaryGChromaticityY = new EbmlElementInfo(0x000055D4, "PrimaryGChromaticityY", EbmlElementType.Float, EbmlElementFlags.None, 6, "Green Y chromaticity coordinate as defined by CIE 1931.");

		/// <summary>Blue X chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly EbmlElementInfo PrimaryBChromaticityX = new EbmlElementInfo(0x000055D5, "PrimaryBChromaticityX", EbmlElementType.Float, EbmlElementFlags.None, 6, "Blue X chromaticity coordinate as defined by CIE 1931.");

		/// <summary>Blue Y chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly EbmlElementInfo PrimaryBChromaticityY = new EbmlElementInfo(0x000055D6, "PrimaryBChromaticityY", EbmlElementType.Float, EbmlElementFlags.None, 6, "Blue Y chromaticity coordinate as defined by CIE 1931.");

		/// <summary>White X chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly EbmlElementInfo WhitePointChromaticityX = new EbmlElementInfo(0x000055D7, "WhitePointChromaticityX", EbmlElementType.Float, EbmlElementFlags.None, 6, "White X chromaticity coordinate as defined by CIE 1931.");

		/// <summary>White Y chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly EbmlElementInfo WhitePointChromaticityY = new EbmlElementInfo(0x000055D8, "WhitePointChromaticityY", EbmlElementType.Float, EbmlElementFlags.None, 6, "White Y chromaticity coordinate as defined by CIE 1931.");

		/// <summary>Maximum luminance. Shall be represented in candelas per square meter (cd/mý).</summary>
		public static readonly EbmlElementInfo LuminanceMax = new EbmlElementInfo(0x000055D9, "LuminanceMax", EbmlElementType.Float, EbmlElementFlags.None, 6, "Maximum luminance. Shall be represented in candelas per square meter (cd/mý).");

		/// <summary>Mininum luminance. Shall be represented in candelas per square meter (cd/mý).</summary>
		public static readonly EbmlElementInfo LuminanceMin = new EbmlElementInfo(0x000055DA, "LuminanceMin", EbmlElementType.Float, EbmlElementFlags.None, 6, "Mininum luminance. Shall be represented in candelas per square meter (cd/mý).");

		/// <summary>Audio settings.</summary>
		public static readonly EbmlElementInfo Audio = new EbmlElementInfo(0x000000E1, "Audio", EbmlElementType.Master, EbmlElementFlags.None, 3, "Audio settings.");

		/// <summary>Sampling frequency in Hz.</summary>
		public static readonly EbmlElementInfo SamplingFrequency = new EbmlElementInfo(0x000000B5, "SamplingFrequency", EbmlElementType.Float, EbmlElementFlags.Mandatory, 4, "Sampling frequency in Hz.");

		/// <summary>Real output sampling frequency in Hz (used for SBR techniques).</summary>
		public static readonly EbmlElementInfo OutputSamplingFrequency = new EbmlElementInfo(0x000078B5, "OutputSamplingFrequency", EbmlElementType.Float, EbmlElementFlags.None, 4, "Real output sampling frequency in Hz (used for SBR techniques).");

		/// <summary>Numbers of channels in the track.</summary>
		public static readonly EbmlElementInfo Channels = new EbmlElementInfo(0x0000009F, "Channels", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "Numbers of channels in the track.");

		/// <summary>Table of horizontal angles for each successive channel, see appendix.</summary>
		public static readonly EbmlElementInfo ChannelPositions = new EbmlElementInfo(0x00007D7B, "ChannelPositions", EbmlElementType.Binary, EbmlElementFlags.None, 4, "Table of horizontal angles for each successive channel, see appendix.");

		/// <summary>Bits per sample, mostly used for PCM.</summary>
		public static readonly EbmlElementInfo BitDepth = new EbmlElementInfo(0x00006264, "BitDepth", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Bits per sample, mostly used for PCM.");

		/// <summary>Operation that needs to be applied on tracks to create this virtual track. For more details look at the Specification Notes on the subject.</summary>
		public static readonly EbmlElementInfo TrackOperation = new EbmlElementInfo(0x000000E2, "TrackOperation", EbmlElementType.Master, EbmlElementFlags.None, 3, "Operation that needs to be applied on tracks to create this virtual track. For more details look at the Specification Notes on the subject.");

		/// <summary>Contains the list of all video plane tracks that need to be combined to create this 3D track</summary>
		public static readonly EbmlElementInfo TrackCombinePlanes = new EbmlElementInfo(0x000000E3, "TrackCombinePlanes", EbmlElementType.Master, EbmlElementFlags.None, 4, "Contains the list of all video plane tracks that need to be combined to create this 3D track");

		/// <summary>Contains a video plane track that need to be combined to create this 3D track</summary>
		public static readonly EbmlElementInfo TrackPlane = new EbmlElementInfo(0x000000E4, "TrackPlane", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 5, "Contains a video plane track that need to be combined to create this 3D track");

		/// <summary>The trackUID number of the track representing the plane.</summary>
		public static readonly EbmlElementInfo TrackPlaneUID = new EbmlElementInfo(0x000000E5, "TrackPlaneUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 6, "The trackUID number of the track representing the plane.");

		/// <summary>The kind of plane this track corresponds to (0: left eye, 1: right eye, 2: background).</summary>
		public static readonly EbmlElementInfo TrackPlaneType = new EbmlElementInfo(0x000000E6, "TrackPlaneType", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 6, "The kind of plane this track corresponds to (0: left eye, 1: right eye, 2: background).");

		/// <summary>Contains the list of all tracks whose Blocks need to be combined to create this virtual track</summary>
		public static readonly EbmlElementInfo TrackJoinBlocks = new EbmlElementInfo(0x000000E9, "TrackJoinBlocks", EbmlElementType.Master, EbmlElementFlags.None, 4, "Contains the list of all tracks whose Blocks need to be combined to create this virtual track");

		/// <summary>The trackUID number of a track whose blocks are used to create this virtual track.</summary>
		public static readonly EbmlElementInfo TrackJoinUID = new EbmlElementInfo(0x000000ED, "TrackJoinUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 5, "The trackUID number of a track whose blocks are used to create this virtual track.");

		/// <summary>DivX trick track extenstions</summary>
		public static readonly EbmlElementInfo TrickTrackUID = new EbmlElementInfo(0x000000C0, "TrickTrackUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "DivX trick track extenstions");

		/// <summary>DivX trick track extenstions</summary>
		public static readonly EbmlElementInfo TrickTrackSegmentUID = new EbmlElementInfo(0x000000C1, "TrickTrackSegmentUID", EbmlElementType.Binary, EbmlElementFlags.None, 3, "DivX trick track extenstions");

		/// <summary>DivX trick track extenstions</summary>
		public static readonly EbmlElementInfo TrickTrackFlag = new EbmlElementInfo(0x000000C6, "TrickTrackFlag", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "DivX trick track extenstions");

		/// <summary>DivX trick track extenstions</summary>
		public static readonly EbmlElementInfo TrickMasterTrackUID = new EbmlElementInfo(0x000000C7, "TrickMasterTrackUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "DivX trick track extenstions");

		/// <summary>DivX trick track extenstions</summary>
		public static readonly EbmlElementInfo TrickMasterTrackSegmentUID = new EbmlElementInfo(0x000000C4, "TrickMasterTrackSegmentUID", EbmlElementType.Binary, EbmlElementFlags.None, 3, "DivX trick track extenstions");

		/// <summary>Settings for several content encoding mechanisms like compression or encryption.</summary>
		public static readonly EbmlElementInfo ContentEncodings = new EbmlElementInfo(0x00006D80, "ContentEncodings", EbmlElementType.Master, EbmlElementFlags.None, 3, "Settings for several content encoding mechanisms like compression or encryption.");

		/// <summary>Settings for one content encoding like compression or encryption.</summary>
		public static readonly EbmlElementInfo ContentEncoding = new EbmlElementInfo(0x00006240, "ContentEncoding", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 4, "Settings for one content encoding like compression or encryption.");

		/// <summary>Tells when this modification was used during encoding/muxing starting with 0 and counting upwards. The decoder/demuxer has to start with the highest order number it finds and work its way down. This value has to be unique over all ContentEncodingOrder Elements in the Segment.</summary>
		public static readonly EbmlElementInfo ContentEncodingOrder = new EbmlElementInfo(0x00005031, "ContentEncodingOrder", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 5, "Tells when this modification was used during encoding/muxing starting with 0 and counting upwards. The decoder/demuxer has to start with the highest order number it finds and work its way down. This value has to be unique over all ContentEncodingOrder Elements in the Segment.");

		/// <summary>A bit field that describes which Elements have been modified in this way. Values (big endian) can be OR'ed. Possible values: 1 - all frame contents, 2 - the track's private data, 4 - the next ContentEncoding (next ContentEncodingOrder. Either the data inside ContentCompression and/or ContentEncryption)</summary>
		public static readonly EbmlElementInfo ContentEncodingScope = new EbmlElementInfo(0x00005032, "ContentEncodingScope", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 5, "A bit field that describes which Elements have been modified in this way. Values (big endian) can be OR'ed. Possible values: 1 - all frame contents, 2 - the track's private data, 4 - the next ContentEncoding (next ContentEncodingOrder. Either the data inside ContentCompression and/or ContentEncryption)");

		/// <summary>A value describing what kind of transformation has been done. Possible values: 0 - compression, 1 - encryption</summary>
		public static readonly EbmlElementInfo ContentEncodingType = new EbmlElementInfo(0x00005033, "ContentEncodingType", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 5, "A value describing what kind of transformation has been done. Possible values: 0 - compression, 1 - encryption");

		/// <summary>Settings describing the compression used. Must be present if the value of ContentEncodingType is 0 and absent otherwise. Each block must be decompressable even if no previous block is available in order not to prevent seeking.</summary>
		public static readonly EbmlElementInfo ContentCompression = new EbmlElementInfo(0x00005034, "ContentCompression", EbmlElementType.Master, EbmlElementFlags.None, 5, "Settings describing the compression used. Must be present if the value of ContentEncodingType is 0 and absent otherwise. Each block must be decompressable even if no previous block is available in order not to prevent seeking.");

		/// <summary>The compression algorithm used. Algorithms that have been specified so far are: 0 - zlib,1 - bzlib,2 - lzo1x 3 - Header Stripping</summary>
		public static readonly EbmlElementInfo ContentCompAlgo = new EbmlElementInfo(0x00004254, "ContentCompAlgo", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 6, "The compression algorithm used. Algorithms that have been specified so far are: 0 - zlib,1 - bzlib,2 - lzo1x 3 - Header Stripping");

		/// <summary>Settings that might be needed by the decompressor. For Header Stripping (ContentCompAlgo=3), the bytes that were removed from the beggining of each frames of the track.</summary>
		public static readonly EbmlElementInfo ContentCompSettings = new EbmlElementInfo(0x00004255, "ContentCompSettings", EbmlElementType.Binary, EbmlElementFlags.None, 6, "Settings that might be needed by the decompressor. For Header Stripping (ContentCompAlgo=3), the bytes that were removed from the beggining of each frames of the track.");

		/// <summary>Settings describing the encryption used. Must be present if the value of ContentEncodingType is 1 and absent otherwise.</summary>
		public static readonly EbmlElementInfo ContentEncryption = new EbmlElementInfo(0x00005035, "ContentEncryption", EbmlElementType.Master, EbmlElementFlags.None, 5, "Settings describing the encryption used. Must be present if the value of ContentEncodingType is 1 and absent otherwise.");

		/// <summary>The encryption algorithm used. The value '0' means that the contents have not been encrypted but only signed. Predefined values: 1 - DES, 2 - 3DES, 3 - Twofish, 4 - Blowfish, 5 - AES</summary>
		public static readonly EbmlElementInfo ContentEncAlgo = new EbmlElementInfo(0x000047E1, "ContentEncAlgo", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 6, "The encryption algorithm used. The value '0' means that the contents have not been encrypted but only signed. Predefined values: 1 - DES, 2 - 3DES, 3 - Twofish, 4 - Blowfish, 5 - AES");

		/// <summary>For public key algorithms this is the ID of the public key the the data was encrypted with.</summary>
		public static readonly EbmlElementInfo ContentEncKeyID = new EbmlElementInfo(0x000047E2, "ContentEncKeyID", EbmlElementType.Binary, EbmlElementFlags.None, 6, "For public key algorithms this is the ID of the public key the the data was encrypted with.");

		/// <summary>A cryptographic signature of the contents.</summary>
		public static readonly EbmlElementInfo ContentSignature = new EbmlElementInfo(0x000047E3, "ContentSignature", EbmlElementType.Binary, EbmlElementFlags.None, 6, "A cryptographic signature of the contents.");

		/// <summary>This is the ID of the private key the data was signed with.</summary>
		public static readonly EbmlElementInfo ContentSigKeyID = new EbmlElementInfo(0x000047E4, "ContentSigKeyID", EbmlElementType.Binary, EbmlElementFlags.None, 6, "This is the ID of the private key the data was signed with.");

		/// <summary>The algorithm used for the signature. A value of '0' means that the contents have not been signed but only encrypted. Predefined values: 1 - RSA</summary>
		public static readonly EbmlElementInfo ContentSigAlgo = new EbmlElementInfo(0x000047E5, "ContentSigAlgo", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 6, "The algorithm used for the signature. A value of '0' means that the contents have not been signed but only encrypted. Predefined values: 1 - RSA");

		/// <summary>The hash algorithm used for the signature. A value of '0' means that the contents have not been signed but only encrypted. Predefined values: 1 - SHA1-160 2 - MD5</summary>
		public static readonly EbmlElementInfo ContentSigHashAlgo = new EbmlElementInfo(0x000047E6, "ContentSigHashAlgo", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 6, "The hash algorithm used for the signature. A value of '0' means that the contents have not been signed but only encrypted. Predefined values: 1 - SHA1-160 2 - MD5");

		/// <summary>A Top-Level Element to speed seeking access. All entries are local to the Segment. Should be mandatory for non "live" streams.</summary>
		public static readonly EbmlElementInfo Cues = new EbmlElementInfo(0x1C53BB6B, "Cues", EbmlElementType.Master, EbmlElementFlags.None, 1, "A Top-Level Element to speed seeking access. All entries are local to the Segment. Should be mandatory for non \"live\" streams.");

		/// <summary>Contains all information relative to a seek point in the Segment.</summary>
		public static readonly EbmlElementInfo CuePoint = new EbmlElementInfo(0x000000BB, "CuePoint", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 2, "Contains all information relative to a seek point in the Segment.");

		/// <summary>Absolute timestamp according to the Segment time base.</summary>
		public static readonly EbmlElementInfo CueTime = new EbmlElementInfo(0x000000B3, "CueTime", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "Absolute timestamp according to the Segment time base.");

		/// <summary>Contain positions for different tracks corresponding to the timestamp.</summary>
		public static readonly EbmlElementInfo CueTrackPositions = new EbmlElementInfo(0x000000B7, "CueTrackPositions", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 3, "Contain positions for different tracks corresponding to the timestamp.");

		/// <summary>The track for which a position is given.</summary>
		public static readonly EbmlElementInfo CueTrack = new EbmlElementInfo(0x000000F7, "CueTrack", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "The track for which a position is given.");

		/// <summary>The position of the Cluster containing the required Block.</summary>
		public static readonly EbmlElementInfo CueClusterPosition = new EbmlElementInfo(0x000000F1, "CueClusterPosition", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "The position of the Cluster containing the required Block.");

		/// <summary>The relative position of the referenced block inside the cluster with 0 being the first possible position for an Element inside that cluster.</summary>
		public static readonly EbmlElementInfo CueRelativePosition = new EbmlElementInfo(0x000000F0, "CueRelativePosition", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "The relative position of the referenced block inside the cluster with 0 being the first possible position for an Element inside that cluster.");

		/// <summary>The duration of the block according to the Segment time base. If missing the track's DefaultDuration does not apply and no duration information is available in terms of the cues.</summary>
		public static readonly EbmlElementInfo CueDuration = new EbmlElementInfo(0x000000B2, "CueDuration", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "The duration of the block according to the Segment time base. If missing the track's DefaultDuration does not apply and no duration information is available in terms of the cues.");

		/// <summary>Number of the Block in the specified Cluster.</summary>
		public static readonly EbmlElementInfo CueBlockNumber = new EbmlElementInfo(0x00005378, "CueBlockNumber", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Number of the Block in the specified Cluster.");

		/// <summary>The position of the Codec State corresponding to this Cue Element. 0 means that the data is taken from the initial Track Entry.</summary>
		public static readonly EbmlElementInfo CueCodecState = new EbmlElementInfo(0x000000EA, "CueCodecState", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "The position of the Codec State corresponding to this Cue Element. 0 means that the data is taken from the initial Track Entry.");

		/// <summary>The Clusters containing the required referenced Blocks.</summary>
		public static readonly EbmlElementInfo CueReference = new EbmlElementInfo(0x000000DB, "CueReference", EbmlElementType.Master, EbmlElementFlags.None, 4, "The Clusters containing the required referenced Blocks.");

		/// <summary>Timestamp of the referenced Block.</summary>
		public static readonly EbmlElementInfo CueRefTime = new EbmlElementInfo(0x00000096, "CueRefTime", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 5, "Timestamp of the referenced Block.");

		/// <summary>The Position of the Cluster containing the referenced Block.</summary>
		public static readonly EbmlElementInfo CueRefCluster = new EbmlElementInfo(0x00000097, "CueRefCluster", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 5, "The Position of the Cluster containing the referenced Block.");

		/// <summary>Number of the referenced Block of Track X in the specified Cluster.</summary>
		public static readonly EbmlElementInfo CueRefNumber = new EbmlElementInfo(0x0000535F, "CueRefNumber", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "Number of the referenced Block of Track X in the specified Cluster.");

		/// <summary>The position of the Codec State corresponding to this referenced Element. 0 means that the data is taken from the initial Track Entry.</summary>
		public static readonly EbmlElementInfo CueRefCodecState = new EbmlElementInfo(0x000000EB, "CueRefCodecState", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 5, "The position of the Codec State corresponding to this referenced Element. 0 means that the data is taken from the initial Track Entry.");

		/// <summary>Contain attached files.</summary>
		public static readonly EbmlElementInfo Attachments = new EbmlElementInfo(0x1941A469, "Attachments", EbmlElementType.Master, EbmlElementFlags.None, 1, "Contain attached files.");

		/// <summary>An attached file.</summary>
		public static readonly EbmlElementInfo AttachedFile = new EbmlElementInfo(0x000061A7, "AttachedFile", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 2, "An attached file.");

		/// <summary>A human-friendly name for the attached file.</summary>
		public static readonly EbmlElementInfo FileDescription = new EbmlElementInfo(0x0000467E, "FileDescription", EbmlElementType.UTF8, EbmlElementFlags.None, 3, "A human-friendly name for the attached file.");

		/// <summary>Filename of the attached file.</summary>
		public static readonly EbmlElementInfo FileName = new EbmlElementInfo(0x0000466E, "FileName", EbmlElementType.UTF8, EbmlElementFlags.Mandatory, 3, "Filename of the attached file.");

		/// <summary>MIME type of the file.</summary>
		public static readonly EbmlElementInfo FileMimeType = new EbmlElementInfo(0x00004660, "FileMimeType", EbmlElementType.String, EbmlElementFlags.Mandatory, 3, "MIME type of the file.");

		/// <summary>The data of the file.</summary>
		public static readonly EbmlElementInfo FileData = new EbmlElementInfo(0x0000465C, "FileData", EbmlElementType.Binary, EbmlElementFlags.Mandatory, 3, "The data of the file.");

		/// <summary>Unique ID representing the file, as random as possible.</summary>
		public static readonly EbmlElementInfo FileUID = new EbmlElementInfo(0x000046AE, "FileUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "Unique ID representing the file, as random as possible.");

		/// <summary>A binary value that a track/codec can refer to when the attachment is needed.</summary>
		public static readonly EbmlElementInfo FileReferral = new EbmlElementInfo(0x00004675, "FileReferral", EbmlElementType.Binary, EbmlElementFlags.None, 3, "A binary value that a track/codec can refer to when the attachment is needed.");

		/// <summary>DivX font extension</summary>
		public static readonly EbmlElementInfo FileUsedStartTime = new EbmlElementInfo(0x00004661, "FileUsedStartTime", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "DivX font extension");

		/// <summary>DivX font extension</summary>
		public static readonly EbmlElementInfo FileUsedEndTime = new EbmlElementInfo(0x00004662, "FileUsedEndTime", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "DivX font extension");

		/// <summary>A system to define basic menus and partition data. For more detailed information, look at the Chapters Explanation.</summary>
		public static readonly EbmlElementInfo Chapters = new EbmlElementInfo(0x1043A770, "Chapters", EbmlElementType.Master, EbmlElementFlags.None, 1, "A system to define basic menus and partition data. For more detailed information, look at the Chapters Explanation.");

		/// <summary>Contains all information about a Segment edition.</summary>
		public static readonly EbmlElementInfo EditionEntry = new EbmlElementInfo(0x000045B9, "EditionEntry", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 2, "Contains all information about a Segment edition.");

		/// <summary>A unique ID to identify the edition. It's useful for tagging an edition.</summary>
		public static readonly EbmlElementInfo EditionUID = new EbmlElementInfo(0x000045BC, "EditionUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "A unique ID to identify the edition. It's useful for tagging an edition.");

		/// <summary>If an edition is hidden (1), it should not be available to the user interface (but still to Control Tracks; see flag notes). (1 bit)</summary>
		public static readonly EbmlElementInfo EditionFlagHidden = new EbmlElementInfo(0x000045BD, "EditionFlagHidden", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "If an edition is hidden (1), it should not be available to the user interface (but still to Control Tracks; see flag notes). (1 bit)");

		/// <summary>If a flag is set (1) the edition should be used as the default one. (1 bit)</summary>
		public static readonly EbmlElementInfo EditionFlagDefault = new EbmlElementInfo(0x000045DB, "EditionFlagDefault", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 3, "If a flag is set (1) the edition should be used as the default one. (1 bit)");

		/// <summary>Specify if the chapters can be defined multiple times and the order to play them is enforced. (1 bit)</summary>
		public static readonly EbmlElementInfo EditionFlagOrdered = new EbmlElementInfo(0x000045DD, "EditionFlagOrdered", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 3, "Specify if the chapters can be defined multiple times and the order to play them is enforced. (1 bit)");

		/// <summary>Contains the atom information to use as the chapter atom (apply to all tracks).</summary>
		public static readonly EbmlElementInfo ChapterAtom = new EbmlElementInfo(0x000000B6, "ChapterAtom", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 3, "Contains the atom information to use as the chapter atom (apply to all tracks).");

		/// <summary>A unique ID to identify the Chapter.</summary>
		public static readonly EbmlElementInfo ChapterUID = new EbmlElementInfo(0x000073C4, "ChapterUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "A unique ID to identify the Chapter.");

		/// <summary>A unique string ID to identify the Chapter. Use for WebVTT cue identifier storage.</summary>
		public static readonly EbmlElementInfo ChapterStringUID = new EbmlElementInfo(0x00005654, "ChapterStringUID", EbmlElementType.UTF8, EbmlElementFlags.None, 4, "A unique string ID to identify the Chapter. Use for WebVTT cue identifier storage.");

		/// <summary>Timestamp of the start of Chapter (not scaled).</summary>
		public static readonly EbmlElementInfo ChapterTimeStart = new EbmlElementInfo(0x00000091, "ChapterTimeStart", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "Timestamp of the start of Chapter (not scaled).");

		/// <summary>Timestamp of the end of Chapter (timestamp excluded, not scaled).</summary>
		public static readonly EbmlElementInfo ChapterTimeEnd = new EbmlElementInfo(0x00000092, "ChapterTimeEnd", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Timestamp of the end of Chapter (timestamp excluded, not scaled).");

		/// <summary>If a chapter is hidden (1), it should not be available to the user interface (but still to Control Tracks; see flag notes). (1 bit)</summary>
		public static readonly EbmlElementInfo ChapterFlagHidden = new EbmlElementInfo(0x00000098, "ChapterFlagHidden", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "If a chapter is hidden (1), it should not be available to the user interface (but still to Control Tracks; see flag notes). (1 bit)");

		/// <summary>Specify wether the chapter is enabled. It can be enabled/disabled by a Control Track. When disabled, the movie should skip all the content between the TimeStart and TimeEnd of this chapter (see flag notes). (1 bit)</summary>
		public static readonly EbmlElementInfo ChapterFlagEnabled = new EbmlElementInfo(0x00004598, "ChapterFlagEnabled", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "Specify wether the chapter is enabled. It can be enabled/disabled by a Control Track. When disabled, the movie should skip all the content between the TimeStart and TimeEnd of this chapter (see flag notes). (1 bit)");

		/// <summary>A Segment to play in place of this chapter. Edition ChapterSegmentEditionUID should be used for this Segment, otherwise no edition is used.</summary>
		public static readonly EbmlElementInfo ChapterSegmentUID = new EbmlElementInfo(0x00006E67, "ChapterSegmentUID", EbmlElementType.Binary, EbmlElementFlags.None, 4, "A Segment to play in place of this chapter. Edition ChapterSegmentEditionUID should be used for this Segment, otherwise no edition is used.");

		/// <summary>The EditionUID to play from the Segment linked in ChapterSegmentUID.</summary>
		public static readonly EbmlElementInfo ChapterSegmentEditionUID = new EbmlElementInfo(0x00006EBC, "ChapterSegmentEditionUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "The EditionUID to play from the Segment linked in ChapterSegmentUID.");

		/// <summary>Specify the physical equivalent of this ChapterAtom like "DVD" (60) or "SIDE" (50), see complete list of values.</summary>
		public static readonly EbmlElementInfo ChapterPhysicalEquiv = new EbmlElementInfo(0x000063C3, "ChapterPhysicalEquiv", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "Specify the physical equivalent of this ChapterAtom like \"DVD\" (60) or \"SIDE\" (50), see complete list of values.");

		/// <summary>List of tracks on which the chapter applies. If this Element is not present, all tracks apply</summary>
		public static readonly EbmlElementInfo ChapterTrack = new EbmlElementInfo(0x0000008F, "ChapterTrack", EbmlElementType.Master, EbmlElementFlags.None, 4, "List of tracks on which the chapter applies. If this Element is not present, all tracks apply");

		/// <summary>UID of the Track to apply this chapter too. In the absence of a control track, choosing this chapter will select the listed Tracks and deselect unlisted tracks. Absence of this Element indicates that the Chapter should be applied to any currently used Tracks.</summary>
		public static readonly EbmlElementInfo ChapterTrackNumber = new EbmlElementInfo(0x00000089, "ChapterTrackNumber", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 5, "UID of the Track to apply this chapter too. In the absence of a control track, choosing this chapter will select the listed Tracks and deselect unlisted tracks. Absence of this Element indicates that the Chapter should be applied to any currently used Tracks.");

		/// <summary>Contains all possible strings to use for the chapter display.</summary>
		public static readonly EbmlElementInfo ChapterDisplay = new EbmlElementInfo(0x00000080, "ChapterDisplay", EbmlElementType.Master, EbmlElementFlags.None, 4, "Contains all possible strings to use for the chapter display.");

		/// <summary>Contains the string to use as the chapter atom.</summary>
		public static readonly EbmlElementInfo ChapString = new EbmlElementInfo(0x00000085, "ChapString", EbmlElementType.UTF8, EbmlElementFlags.Mandatory, 5, "Contains the string to use as the chapter atom.");

		/// <summary>The languages corresponding to the string, in the bibliographic ISO-639-2 form.</summary>
		public static readonly EbmlElementInfo ChapLanguage = new EbmlElementInfo(0x0000437C, "ChapLanguage", EbmlElementType.String, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 5, "The languages corresponding to the string, in the bibliographic ISO-639-2 form.");

		/// <summary>The countries corresponding to the string, same 2 octets as in Internet domains.</summary>
		public static readonly EbmlElementInfo ChapCountry = new EbmlElementInfo(0x0000437E, "ChapCountry", EbmlElementType.String, EbmlElementFlags.None, 5, "The countries corresponding to the string, same 2 octets as in Internet domains.");

		/// <summary>Contains all the commands associated to the Atom.</summary>
		public static readonly EbmlElementInfo ChapProcess = new EbmlElementInfo(0x00006944, "ChapProcess", EbmlElementType.Master, EbmlElementFlags.None, 4, "Contains all the commands associated to the Atom.");

		/// <summary>Contains the type of the codec used for the processing. A value of 0 means native Matroska processing (to be defined), a value of 1 means the DVD command set is used. More codec IDs can be added later.</summary>
		public static readonly EbmlElementInfo ChapProcessCodecID = new EbmlElementInfo(0x00006955, "ChapProcessCodecID", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 5, "Contains the type of the codec used for the processing. A value of 0 means native Matroska processing (to be defined), a value of 1 means the DVD command set is used. More codec IDs can be added later.");

		/// <summary>Some optional data attached to the ChapProcessCodecID information. For ChapProcessCodecID = 1, it is the "DVD level" equivalent.</summary>
		public static readonly EbmlElementInfo ChapProcessPrivate = new EbmlElementInfo(0x0000450D, "ChapProcessPrivate", EbmlElementType.Binary, EbmlElementFlags.None, 5, "Some optional data attached to the ChapProcessCodecID information. For ChapProcessCodecID = 1, it is the \"DVD level\" equivalent.");

		/// <summary>Contains all the commands associated to the Atom.</summary>
		public static readonly EbmlElementInfo ChapProcessCommand = new EbmlElementInfo(0x00006911, "ChapProcessCommand", EbmlElementType.Master, EbmlElementFlags.None, 5, "Contains all the commands associated to the Atom.");

		/// <summary>Defines when the process command should be handled (0: during the whole chapter, 1: before starting playback, 2: after playback of the chapter).</summary>
		public static readonly EbmlElementInfo ChapProcessTime = new EbmlElementInfo(0x00006922, "ChapProcessTime", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 6, "Defines when the process command should be handled (0: during the whole chapter, 1: before starting playback, 2: after playback of the chapter).");

		/// <summary>Contains the command information. The data should be interpreted depending on the ChapProcessCodecID value. For ChapProcessCodecID = 1, the data correspond to the binary DVD cell pre/post commands.</summary>
		public static readonly EbmlElementInfo ChapProcessData = new EbmlElementInfo(0x00006933, "ChapProcessData", EbmlElementType.Binary, EbmlElementFlags.Mandatory, 6, "Contains the command information. The data should be interpreted depending on the ChapProcessCodecID value. For ChapProcessCodecID = 1, the data correspond to the binary DVD cell pre/post commands.");

		/// <summary>Element containing Elements specific to Tracks/Chapters. A list of valid tags can be found here.</summary>
		public static readonly EbmlElementInfo Tags = new EbmlElementInfo(0x1254C367, "Tags", EbmlElementType.Master, EbmlElementFlags.None, 1, "Element containing Elements specific to Tracks/Chapters. A list of valid tags can be found here.");

		/// <summary>Element containing Elements specific to Tracks/Chapters.</summary>
		public static readonly EbmlElementInfo Tag = new EbmlElementInfo(0x00007373, "Tag", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 2, "Element containing Elements specific to Tracks/Chapters.");

		/// <summary>Contain all UIDs where the specified meta data apply. It is empty to describe everything in the Segment.</summary>
		public static readonly EbmlElementInfo Targets = new EbmlElementInfo(0x000063C0, "Targets", EbmlElementType.Master, EbmlElementFlags.Mandatory, 3, "Contain all UIDs where the specified meta data apply. It is empty to describe everything in the Segment.");

		/// <summary>A number to indicate the logical level of the target (see TargetType).</summary>
		public static readonly EbmlElementInfo TargetTypeValue = new EbmlElementInfo(0x000068CA, "TargetTypeValue", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "A number to indicate the logical level of the target (see TargetType).");

		/// <summary>An informational string that can be used to display the logical level of the target like "ALBUM", "TRACK", "MOVIE", "CHAPTER", etc (see TargetType).</summary>
		public static readonly EbmlElementInfo TargetType = new EbmlElementInfo(0x000063CA, "TargetType", EbmlElementType.String, EbmlElementFlags.None, 4, "An informational string that can be used to display the logical level of the target like \"ALBUM\", \"TRACK\", \"MOVIE\", \"CHAPTER\", etc (see TargetType).");

		/// <summary>A unique ID to identify the Track(s) the tags belong to. If the value is 0 at this level, the tags apply to all tracks in the Segment.</summary>
		public static readonly EbmlElementInfo TagTrackUID = new EbmlElementInfo(0x000063C5, "TagTrackUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "A unique ID to identify the Track(s) the tags belong to. If the value is 0 at this level, the tags apply to all tracks in the Segment.");

		/// <summary>A unique ID to identify the EditionEntry(s) the tags belong to. If the value is 0 at this level, the tags apply to all editions in the Segment.</summary>
		public static readonly EbmlElementInfo TagEditionUID = new EbmlElementInfo(0x000063C9, "TagEditionUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "A unique ID to identify the EditionEntry(s) the tags belong to. If the value is 0 at this level, the tags apply to all editions in the Segment.");

		/// <summary>A unique ID to identify the Chapter(s) the tags belong to. If the value is 0 at this level, the tags apply to all chapters in the Segment.</summary>
		public static readonly EbmlElementInfo TagChapterUID = new EbmlElementInfo(0x000063C4, "TagChapterUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "A unique ID to identify the Chapter(s) the tags belong to. If the value is 0 at this level, the tags apply to all chapters in the Segment.");

		/// <summary>A unique ID to identify the Attachment(s) the tags belong to. If the value is 0 at this level, the tags apply to all the attachments in the Segment.</summary>
		public static readonly EbmlElementInfo TagAttachmentUID = new EbmlElementInfo(0x000063C6, "TagAttachmentUID", EbmlElementType.UnsignedInteger, EbmlElementFlags.None, 4, "A unique ID to identify the Attachment(s) the tags belong to. If the value is 0 at this level, the tags apply to all the attachments in the Segment.");

		/// <summary>Contains general information about the target.</summary>
		public static readonly EbmlElementInfo SimpleTag = new EbmlElementInfo(0x000067C8, "SimpleTag", EbmlElementType.Master, EbmlElementFlags.Mandatory | EbmlElementFlags.Multiple, 3, "Contains general information about the target.");

		/// <summary>The name of the Tag that is going to be stored.</summary>
		public static readonly EbmlElementInfo TagName = new EbmlElementInfo(0x000045A3, "TagName", EbmlElementType.UTF8, EbmlElementFlags.Mandatory, 4, "The name of the Tag that is going to be stored.");

		/// <summary>Specifies the language of the tag specified, in the Matroska languages form.</summary>
		public static readonly EbmlElementInfo TagLanguage = new EbmlElementInfo(0x0000447A, "TagLanguage", EbmlElementType.String, EbmlElementFlags.Mandatory, 4, "Specifies the language of the tag specified, in the Matroska languages form.");

		/// <summary>Indication to know if this is the default/original language to use for the given tag. (1 bit)</summary>
		public static readonly EbmlElementInfo TagDefault = new EbmlElementInfo(0x00004484, "TagDefault", EbmlElementType.UnsignedInteger, EbmlElementFlags.Mandatory, 4, "Indication to know if this is the default/original language to use for the given tag. (1 bit)");

		/// <summary>The value of the Tag.</summary>
		public static readonly EbmlElementInfo TagString = new EbmlElementInfo(0x00004487, "TagString", EbmlElementType.UTF8, EbmlElementFlags.None, 4, "The value of the Tag.");

		/// <summary>The values of the Tag if it is binary. Note that this cannot be used in the same SimpleTag as TagString.</summary>
		public static readonly EbmlElementInfo TagBinary = new EbmlElementInfo(0x00004485, "TagBinary", EbmlElementType.Binary, EbmlElementFlags.None, 4, "The values of the Tag if it is binary. Note that this cannot be used in the same SimpleTag as TagString.");

		#endregion
	}
}
