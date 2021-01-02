using System.Collections.Generic;
using System.Reflection;
using NEbml.Core;

namespace Matroska
{
	/// <summary>
	/// Contains the EBML elements specified by the Matroska project.
	/// </summary>
	/// <remarks>
	/// See https://matroska.org/technical/specs/index.html for more info.
	/// </remarks>
	public static class MatroskaSpecification2
	{
		#region Helper

		private static readonly Dictionary<VInt, ElementDescriptor> _elementDescriptors = new Dictionary<VInt, ElementDescriptor>();

		/// <summary>
		/// Gets a dictionary of all Matroska elements.
		/// </summary>
		public static IReadOnlyDictionary<VInt, ElementDescriptor> ElementDescriptors => _elementDescriptors;

		static MatroskaSpecification2()
		{
			var fields = typeof(MatroskaSpecification2).GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo field in fields)
			{
				var elementDescriptor = (ElementDescriptor)field.GetValue(null);
				_elementDescriptors.Add(elementDescriptor.Identifier, elementDescriptor);
			}
		}

		#endregion

		#region Definitions

		/// <summary>Set the EBML characteristics of the data to follow. Each EBML document has to start with this.</summary>
		public static readonly ElementDescriptor EBML = new ElementDescriptor(0x1A45DFA3, "EBML", ElementType.MasterElement);

		/// <summary>The version of EBML parser used to create the file.</summary>
		public static readonly ElementDescriptor EBMLVersion = new ElementDescriptor(0x00004286, "EBMLVersion", ElementType.UnsignedInteger);

		/// <summary>The minimum EBML version a parser has to support to read this file.</summary>
		public static readonly ElementDescriptor EBMLReadVersion = new ElementDescriptor(0x000042F7, "EBMLReadVersion", ElementType.UnsignedInteger);

		/// <summary>The maximum length of the IDs you'll find in this file (4 or less in Matroska).</summary>
		public static readonly ElementDescriptor EBMLMaxIDLength = new ElementDescriptor(0x000042F2, "EBMLMaxIDLength", ElementType.UnsignedInteger);

		/// <summary>The maximum length of the sizes you'll find in this file (8 or less in Matroska). This does not override the Element size indicated at the beginning of an Element. Elements that have an indicated size which is larger than what is allowed by EBMLMaxSizeLength shall be considered invalid.</summary>
		public static readonly ElementDescriptor EBMLMaxSizeLength = new ElementDescriptor(0x000042F3, "EBMLMaxSizeLength", ElementType.UnsignedInteger);

		/// <summary>A string that describes the type of document that follows this EBML header. 'matroska' in our case or 'webm' for webm files.</summary>
		public static readonly ElementDescriptor DocType = new ElementDescriptor(0x00004282, "DocType", ElementType.AsciiString);

		/// <summary>The version of DocType interpreter used to create the file.</summary>
		public static readonly ElementDescriptor DocTypeVersion = new ElementDescriptor(0x00004287, "DocTypeVersion", ElementType.UnsignedInteger);

		/// <summary>The minimum DocType version an interpreter has to support to read this file.</summary>
		public static readonly ElementDescriptor DocTypeReadVersion = new ElementDescriptor(0x00004285, "DocTypeReadVersion", ElementType.UnsignedInteger);

		/// <summary>Used to void damaged data, to avoid unexpected behaviors when using damaged data. The content is discarded. Also used to reserve space in a sub-element for later use.</summary>
		public static readonly ElementDescriptor Void = new ElementDescriptor(0x000000EC, "Void", ElementType.Binary);

		/// <summary>The CRC is computed on all the data of the Master-element it's in. The CRC Element should be the first in it's parent master for easier reading. All level 1 Elements should include a CRC-32. The CRC in use is the IEEE CRC32 Little Endian</summary>
		public static readonly ElementDescriptor CRC32 = new ElementDescriptor(0x000000BF, "CRC-32", ElementType.Binary);

		/// <summary>Contain signature of some (coming) Elements in the stream.</summary>
		public static readonly ElementDescriptor SignatureSlot = new ElementDescriptor(0x1B538667, "SignatureSlot", ElementType.MasterElement);

		/// <summary>Signature algorithm used (1=RSA, 2=elliptic).</summary>
		public static readonly ElementDescriptor SignatureAlgo = new ElementDescriptor(0x00007E8A, "SignatureAlgo", ElementType.UnsignedInteger);

		/// <summary>Hash algorithm used (1=SHA1-160, 2=MD5).</summary>
		public static readonly ElementDescriptor SignatureHash = new ElementDescriptor(0x00007E9A, "SignatureHash", ElementType.UnsignedInteger);

		/// <summary>The public key to use with the algorithm (in the case of a PKI-based signature).</summary>
		public static readonly ElementDescriptor SignaturePublicKey = new ElementDescriptor(0x00007EA5, "SignaturePublicKey", ElementType.Binary);

		/// <summary>The signature of the data (until a new.</summary>
		public static readonly ElementDescriptor Signature = new ElementDescriptor(0x00007EB5, "Signature", ElementType.Binary);

		/// <summary>Contains Elements that will be used to compute the signature.</summary>
		public static readonly ElementDescriptor SignatureElements = new ElementDescriptor(0x00007E5B, "SignatureElements", ElementType.MasterElement);

		/// <summary>A list consists of a number of consecutive Elements that represent one case where data is used in signature. Ex: Cluster|Block|BlockAdditional means that the BlockAdditional of all Blocks in all Clusters is used for encryption.</summary>
		public static readonly ElementDescriptor SignatureElementList = new ElementDescriptor(0x00007E7B, "SignatureElementList", ElementType.MasterElement);

		/// <summary>An Element ID whose data will be used to compute the signature.</summary>
		public static readonly ElementDescriptor SignedElement = new ElementDescriptor(0x00006532, "SignedElement", ElementType.Binary);

		/// <summary>The Root Element that contains all other Top-Level Elements (Elements defined only at Level 1). A Matroska file is composed of 1 Segment.</summary>
		public static readonly ElementDescriptor Segment = new ElementDescriptor(0x18538067, "Segment", ElementType.MasterElement);

		/// <summary>Contains the position of other Top-Level Elements.</summary>
		public static readonly ElementDescriptor SeekHead = new ElementDescriptor(0x114D9B74, "SeekHead", ElementType.MasterElement);

		/// <summary>Contains a single seek entry to an EBML Element.</summary>
		public static readonly ElementDescriptor Seek = new ElementDescriptor(0x00004DBB, "Seek", ElementType.MasterElement);

		/// <summary>The binary ID corresponding to the Element name.</summary>
		public static readonly ElementDescriptor SeekID = new ElementDescriptor(0x000053AB, "SeekID", ElementType.Binary);

		/// <summary>The position of the Element in the Segment in octets (0 = first level 1 Element).</summary>
		public static readonly ElementDescriptor SeekPosition = new ElementDescriptor(0x000053AC, "SeekPosition", ElementType.UnsignedInteger);

		/// <summary>Contains miscellaneous general information and statistics on the file.</summary>
		public static readonly ElementDescriptor Info = new ElementDescriptor(0x1549A966, "Info", ElementType.MasterElement);

		/// <summary>A randomly generated unique ID to identify the current Segment between many others (128 bits).</summary>
		public static readonly ElementDescriptor SegmentUID = new ElementDescriptor(0x000073A4, "SegmentUID", ElementType.Binary);

		/// <summary>A filename corresponding to this Segment.</summary>
		public static readonly ElementDescriptor SegmentFilename = new ElementDescriptor(0x00007384, "SegmentFilename", ElementType.Utf8String);

		/// <summary>A unique ID to identify the previous chained Segment (128 bits).</summary>
		public static readonly ElementDescriptor PrevUID = new ElementDescriptor(0x003CB923, "PrevUID", ElementType.Binary);

		/// <summary>An escaped filename corresponding to the previous Segment.</summary>
		public static readonly ElementDescriptor PrevFilename = new ElementDescriptor(0x003C83AB, "PrevFilename", ElementType.Utf8String);

		/// <summary>A unique ID to identify the next chained Segment (128 bits).</summary>
		public static readonly ElementDescriptor NextUID = new ElementDescriptor(0x003EB923, "NextUID", ElementType.Binary);

		/// <summary>An escaped filename corresponding to the next Segment.</summary>
		public static readonly ElementDescriptor NextFilename = new ElementDescriptor(0x003E83BB, "NextFilename", ElementType.Utf8String);

		/// <summary>A randomly generated unique ID that all Segments related to each other must use (128 bits).</summary>
		public static readonly ElementDescriptor SegmentFamily = new ElementDescriptor(0x00004444, "SegmentFamily", ElementType.Binary);

		/// <summary>A tuple of corresponding ID used by chapter codecs to represent this Segment.</summary>
		public static readonly ElementDescriptor ChapterTranslate = new ElementDescriptor(0x00006924, "ChapterTranslate", ElementType.MasterElement);

		/// <summary>Specify an edition UID on which this correspondance applies. When not specified, it means for all editions found in the Segment.</summary>
		public static readonly ElementDescriptor ChapterTranslateEditionUID = new ElementDescriptor(0x000069FC, "ChapterTranslateEditionUID", ElementType.UnsignedInteger);

		/// <summary>The chapter codec using this ID (0: Matroska Script, 1: DVD-menu).</summary>
		public static readonly ElementDescriptor ChapterTranslateCodec = new ElementDescriptor(0x000069BF, "ChapterTranslateCodec", ElementType.UnsignedInteger);

		/// <summary>The binary value used to represent this Segment in the chapter codec data. The format depends on the ChapProcessCodecID used.</summary>
		public static readonly ElementDescriptor ChapterTranslateID = new ElementDescriptor(0x000069A5, "ChapterTranslateID", ElementType.Binary);

		/// <summary>Timestamp scale in nanoseconds (1.000.000 means all timestamps in the Segment are expressed in milliseconds).</summary>
		public static readonly ElementDescriptor TimecodeScale = new ElementDescriptor(0x002AD7B1, "TimecodeScale", ElementType.UnsignedInteger);

		/// <summary>Duration of the Segment (based on TimecodeScale).</summary>
		public static readonly ElementDescriptor Duration = new ElementDescriptor(0x00004489, "Duration", ElementType.Float);

		/// <summary>Date of the origin of timestamp (value 0), i.e. production date.</summary>
		public static readonly ElementDescriptor DateUTC = new ElementDescriptor(0x00004461, "DateUTC", ElementType.Date);

		/// <summary>General name of the Segment.</summary>
		public static readonly ElementDescriptor Title = new ElementDescriptor(0x00007BA9, "Title", ElementType.Utf8String);

		/// <summary>Muxing application or library ("libmatroska-0.4.3").</summary>
		public static readonly ElementDescriptor MuxingApp = new ElementDescriptor(0x00004D80, "MuxingApp", ElementType.Utf8String);

		/// <summary>Writing application ("mkvmerge-0.3.3").</summary>
		public static readonly ElementDescriptor WritingApp = new ElementDescriptor(0x00005741, "WritingApp", ElementType.Utf8String);

		/// <summary>The Top-Level Element containing the (monolithic) Block structure.</summary>
		public static readonly ElementDescriptor Cluster = new ElementDescriptor(0x1F43B675, "Cluster", ElementType.MasterElement);

		/// <summary>Absolute timestamp of the cluster (based on TimecodeScale).</summary>
		public static readonly ElementDescriptor Timecode = new ElementDescriptor(0x000000E7, "Timecode", ElementType.UnsignedInteger);

		/// <summary>The list of tracks that are not used in that part of the stream. It is useful when using overlay tracks on seeking. Then you should decide what track to use.</summary>
		public static readonly ElementDescriptor SilentTracks = new ElementDescriptor(0x00005854, "SilentTracks", ElementType.MasterElement);

		/// <summary>One of the track number that are not used from now on in the stream. It could change later if not specified as silent in a further Cluster.</summary>
		public static readonly ElementDescriptor SilentTrackNumber = new ElementDescriptor(0x000058D7, "SilentTrackNumber", ElementType.UnsignedInteger);

		/// <summary>The Position of the Cluster in the Segment (0 in live broadcast streams). It might help to resynchronise offset on damaged streams.</summary>
		public static readonly ElementDescriptor Position = new ElementDescriptor(0x000000A7, "Position", ElementType.UnsignedInteger);

		/// <summary>Size of the previous Cluster, in octets. Can be useful for backward playing.</summary>
		public static readonly ElementDescriptor PrevSize = new ElementDescriptor(0x000000AB, "PrevSize", ElementType.UnsignedInteger);

		/// <summary>Similar to Block but without all the extra information, mostly used to reduced overhead when no extra feature is needed. (see SimpleBlock Structure)</summary>
		public static readonly ElementDescriptor SimpleBlock = new ElementDescriptor(0x000000A3, "SimpleBlock", ElementType.Binary);

		/// <summary>Basic container of information containing a single Block or BlockVirtual, and information specific to that Block/VirtualBlock.</summary>
		public static readonly ElementDescriptor BlockGroup = new ElementDescriptor(0x000000A0, "BlockGroup", ElementType.MasterElement);

		/// <summary>Block containing the actual data to be rendered and a timestamp relative to the Cluster Timecode. (see Block Structure)</summary>
		public static readonly ElementDescriptor Block = new ElementDescriptor(0x000000A1, "Block", ElementType.Binary);

		/// <summary>A Block with no data. It must be stored in the stream at the place the real Block should be in display order. (see Block Virtual)</summary>
		public static readonly ElementDescriptor BlockVirtual = new ElementDescriptor(0x000000A2, "BlockVirtual", ElementType.Binary);

		/// <summary>Contain additional blocks to complete the main one. An EBML parser that has no knowledge of the Block structure could still see and use/skip these data.</summary>
		public static readonly ElementDescriptor BlockAdditions = new ElementDescriptor(0x000075A1, "BlockAdditions", ElementType.MasterElement);

		/// <summary>Contain the BlockAdditional and some parameters.</summary>
		public static readonly ElementDescriptor BlockMore = new ElementDescriptor(0x000000A6, "BlockMore", ElementType.MasterElement);

		/// <summary>An ID to identify the BlockAdditional level.</summary>
		public static readonly ElementDescriptor BlockAddID = new ElementDescriptor(0x000000EE, "BlockAddID", ElementType.UnsignedInteger);

		/// <summary>Interpreted by the codec as it wishes (using the BlockAddID).</summary>
		public static readonly ElementDescriptor BlockAdditional = new ElementDescriptor(0x000000A5, "BlockAdditional", ElementType.Binary);

		/// <summary>The duration of the Block (based on TimecodeScale). This Element is mandatory when DefaultDuration is set for the track (but can be omitted as other default values). When not written and with no DefaultDuration, the value is assumed to be the difference between the timestamp of this Block and the timestamp of the next Block in "display" order (not coding order). This Element can be useful at the end of a Track (as there is not other Block available), or when there is a break in a track like for subtitle tracks. When set to 0 that means the frame is not a keyframe.</summary>
		public static readonly ElementDescriptor BlockDuration = new ElementDescriptor(0x0000009B, "BlockDuration", ElementType.UnsignedInteger);

		/// <summary>This frame is referenced and has the specified cache priority. In cache only a frame of the same or higher priority can replace this frame. A value of 0 means the frame is not referenced.</summary>
		public static readonly ElementDescriptor ReferencePriority = new ElementDescriptor(0x000000FA, "ReferencePriority", ElementType.UnsignedInteger);

		/// <summary>Timestamp of another frame used as a reference (ie: B or P frame). The timestamp is relative to the block it's attached to.</summary>
		public static readonly ElementDescriptor ReferenceBlock = new ElementDescriptor(0x000000FB, "ReferenceBlock", ElementType.SignedInteger);

		/// <summary>Relative position of the data that should be in position of the virtual block.</summary>
		public static readonly ElementDescriptor ReferenceVirtual = new ElementDescriptor(0x000000FD, "ReferenceVirtual", ElementType.SignedInteger);

		/// <summary>The new codec state to use. Data interpretation is private to the codec. This information should always be referenced by a seek entry.</summary>
		public static readonly ElementDescriptor CodecState = new ElementDescriptor(0x000000A4, "CodecState", ElementType.Binary);

		/// <summary>Duration in nanoseconds of the silent data added to the Block (padding at the end of the Block for positive value, at the beginning of the Block for negative value). The duration of DiscardPadding is not calculated in the duration of the TrackEntry and should be discarded during playback.</summary>
		public static readonly ElementDescriptor DiscardPadding = new ElementDescriptor(0x000075A2, "DiscardPadding", ElementType.SignedInteger);

		/// <summary>Contains slices description.</summary>
		public static readonly ElementDescriptor Slices = new ElementDescriptor(0x0000008E, "Slices", ElementType.MasterElement);

		/// <summary>Contains extra time information about the data contained in the Block. While there are a few files in the wild with this Element, it is no longer in use and has been deprecated. Being able to interpret this Element is not required for playback.</summary>
		public static readonly ElementDescriptor TimeSlice = new ElementDescriptor(0x000000E8, "TimeSlice", ElementType.MasterElement);

		/// <summary>The reverse number of the frame in the lace (0 is the last frame, 1 is the next to last, etc). While there are a few files in the wild with this Element, it is no longer in use and has been deprecated. Being able to interpret this Element is not required for playback.</summary>
		public static readonly ElementDescriptor LaceNumber = new ElementDescriptor(0x000000CC, "LaceNumber", ElementType.UnsignedInteger);

		/// <summary>The number of the frame to generate from this lace with this delay (allow you to generate many frames from the same Block/Frame).</summary>
		public static readonly ElementDescriptor FrameNumber = new ElementDescriptor(0x000000CD, "FrameNumber", ElementType.UnsignedInteger);

		/// <summary>The ID of the BlockAdditional Element (0 is the main Block).</summary>
		public static readonly ElementDescriptor BlockAdditionID = new ElementDescriptor(0x000000CB, "BlockAdditionID", ElementType.UnsignedInteger);

		/// <summary>The (scaled) delay to apply to the Element.</summary>
		public static readonly ElementDescriptor Delay = new ElementDescriptor(0x000000CE, "Delay", ElementType.UnsignedInteger);

		/// <summary>The (scaled) duration to apply to the Element.</summary>
		public static readonly ElementDescriptor SliceDuration = new ElementDescriptor(0x000000CF, "SliceDuration", ElementType.UnsignedInteger);

		/// <summary>DivX trick track extenstions</summary>
		public static readonly ElementDescriptor ReferenceFrame = new ElementDescriptor(0x000000C8, "ReferenceFrame", ElementType.MasterElement);

		/// <summary>DivX trick track extenstions</summary>
		public static readonly ElementDescriptor ReferenceOffset = new ElementDescriptor(0x000000C9, "ReferenceOffset", ElementType.UnsignedInteger);

		/// <summary>DivX trick track extenstions</summary>
		public static readonly ElementDescriptor ReferenceTimeCode = new ElementDescriptor(0x000000CA, "ReferenceTimeCode", ElementType.UnsignedInteger);

		/// <summary>Similar to SimpleBlock but the data inside the Block are Transformed (encrypt and/or signed). (see EncryptedBlock Structure)</summary>
		public static readonly ElementDescriptor EncryptedBlock = new ElementDescriptor(0x000000AF, "EncryptedBlock", ElementType.Binary);

		/// <summary>A Top-Level Element of information with many tracks described.</summary>
		public static readonly ElementDescriptor Tracks = new ElementDescriptor(0x1654AE6B, "Tracks", ElementType.MasterElement);

		/// <summary>Describes a track with all Elements.</summary>
		public static readonly ElementDescriptor TrackEntry = new ElementDescriptor(0x000000AE, "TrackEntry", ElementType.MasterElement);

		/// <summary>The track number as used in the Block Header (using more than 127 tracks is not encouraged, though the design allows an unlimited number).</summary>
		public static readonly ElementDescriptor TrackNumber = new ElementDescriptor(0x000000D7, "TrackNumber", ElementType.UnsignedInteger);

		/// <summary>A unique ID to identify the Track. This should be kept the same when making a direct stream copy of the Track to another file.</summary>
		public static readonly ElementDescriptor TrackUID = new ElementDescriptor(0x000073C5, "TrackUID", ElementType.UnsignedInteger);

		/// <summary>A set of track types coded on 8 bits (1: video, 2: audio, 3: complex, 0x10: logo, 0x11: subtitle, 0x12: buttons, 0x20: control).</summary>
		public static readonly ElementDescriptor TrackType = new ElementDescriptor(0x00000083, "TrackType", ElementType.UnsignedInteger);

		/// <summary>Set if the track is usable. (1 bit)</summary>
		public static readonly ElementDescriptor FlagEnabled = new ElementDescriptor(0x000000B9, "FlagEnabled", ElementType.UnsignedInteger);

		/// <summary>Set if that track (audio, video or subs) SHOULD be active if no language found matches the user preference. (1 bit)</summary>
		public static readonly ElementDescriptor FlagDefault = new ElementDescriptor(0x00000088, "FlagDefault", ElementType.UnsignedInteger);

		/// <summary>Set if that track MUST be active during playback. There can be many forced track for a kind (audio, video or subs), the player should select the one which language matches the user preference or the default + forced track. Overlay MAY happen between a forced and non-forced track of the same kind. (1 bit)</summary>
		public static readonly ElementDescriptor FlagForced = new ElementDescriptor(0x000055AA, "FlagForced", ElementType.UnsignedInteger);

		/// <summary>Set if the track may contain blocks using lacing. (1 bit)</summary>
		public static readonly ElementDescriptor FlagLacing = new ElementDescriptor(0x0000009C, "FlagLacing", ElementType.UnsignedInteger);

		/// <summary>The minimum number of frames a player should be able to cache during playback. If set to 0, the reference pseudo-cache system is not used.</summary>
		public static readonly ElementDescriptor MinCache = new ElementDescriptor(0x00006DE7, "MinCache", ElementType.UnsignedInteger);

		/// <summary>The maximum cache size required to store referenced frames in and the current frame. 0 means no cache is needed.</summary>
		public static readonly ElementDescriptor MaxCache = new ElementDescriptor(0x00006DF8, "MaxCache", ElementType.UnsignedInteger);

		/// <summary>Number of nanoseconds (not scaled via TimecodeScale) per frame ('frame' in the Matroska sense -- one Element put into a (Simple)Block).</summary>
		public static readonly ElementDescriptor DefaultDuration = new ElementDescriptor(0x0023E383, "DefaultDuration", ElementType.UnsignedInteger);

		/// <summary>The period in nanoseconds (not scaled by TimcodeScale) between two successive fields at the output of the decoding process (see the notes)</summary>
		public static readonly ElementDescriptor DefaultDecodedFieldDuration = new ElementDescriptor(0x00234E7A, "DefaultDecodedFieldDuration", ElementType.UnsignedInteger);

		/// <summary>DEPRECATED, DO NOT USE. The scale to apply on this track to work at normal speed in relation with other tracks (mostly used to adjust video speed when the audio length differs).</summary>
		public static readonly ElementDescriptor TrackTimecodeScale = new ElementDescriptor(0x0023314F, "TrackTimecodeScale", ElementType.Float);

		/// <summary>A value to add to the Block's Timestamp. This can be used to adjust the playback offset of a track.</summary>
		public static readonly ElementDescriptor TrackOffset = new ElementDescriptor(0x0000537F, "TrackOffset", ElementType.SignedInteger);

		/// <summary>The maximum value of BlockAddID. A value 0 means there is no BlockAdditions for this track.</summary>
		public static readonly ElementDescriptor MaxBlockAdditionID = new ElementDescriptor(0x000055EE, "MaxBlockAdditionID", ElementType.UnsignedInteger);

		/// <summary>A human-readable track name.</summary>
		public static readonly ElementDescriptor Name = new ElementDescriptor(0x0000536E, "Name", ElementType.Utf8String);

		/// <summary>Specifies the language of the track in the Matroska languages form.</summary>
		public static readonly ElementDescriptor Language = new ElementDescriptor(0x0022B59C, "Language", ElementType.AsciiString);

		/// <summary>An ID corresponding to the codec, see the codec page for more info.</summary>
		public static readonly ElementDescriptor CodecID = new ElementDescriptor(0x00000086, "CodecID", ElementType.AsciiString);

		/// <summary>Private data only known to the codec.</summary>
		public static readonly ElementDescriptor CodecPrivate = new ElementDescriptor(0x000063A2, "CodecPrivate", ElementType.Binary);

		/// <summary>A human-readable string specifying the codec.</summary>
		public static readonly ElementDescriptor CodecName = new ElementDescriptor(0x00258688, "CodecName", ElementType.Utf8String);

		/// <summary>The UID of an attachment that is used by this codec.</summary>
		public static readonly ElementDescriptor AttachmentLink = new ElementDescriptor(0x00007446, "AttachmentLink", ElementType.UnsignedInteger);

		/// <summary>A string describing the encoding setting used.</summary>
		public static readonly ElementDescriptor CodecSettings = new ElementDescriptor(0x003A9697, "CodecSettings", ElementType.Utf8String);

		/// <summary>A URL to find information about the codec used.</summary>
		public static readonly ElementDescriptor CodecInfoURL = new ElementDescriptor(0x003B4040, "CodecInfoURL", ElementType.AsciiString);

		/// <summary>A URL to download about the codec used.</summary>
		public static readonly ElementDescriptor CodecDownloadURL = new ElementDescriptor(0x0026B240, "CodecDownloadURL", ElementType.AsciiString);

		/// <summary>The codec can decode potentially damaged data (1 bit).</summary>
		public static readonly ElementDescriptor CodecDecodeAll = new ElementDescriptor(0x000000AA, "CodecDecodeAll", ElementType.UnsignedInteger);

		/// <summary>Specify that this track is an overlay track for the Track specified (in the u-integer). That means when this track has a gap (see SilentTracks) the overlay track should be used instead. The order of multiple TrackOverlay matters, the first one is the one that should be used. If not found it should be the second, etc.</summary>
		public static readonly ElementDescriptor TrackOverlay = new ElementDescriptor(0x00006FAB, "TrackOverlay", ElementType.UnsignedInteger);

		/// <summary>CodecDelay is The codec-built-in delay in nanoseconds. This value must be subtracted from each block timestamp in order to get the actual timestamp. The value should be small so the muxing of tracks with the same actual timestamp are in the same Cluster.</summary>
		public static readonly ElementDescriptor CodecDelay = new ElementDescriptor(0x000056AA, "CodecDelay", ElementType.UnsignedInteger);

		/// <summary>After a discontinuity, SeekPreRoll is the duration in nanoseconds of the data the decoder must decode before the decoded data is valid.</summary>
		public static readonly ElementDescriptor SeekPreRoll = new ElementDescriptor(0x000056BB, "SeekPreRoll", ElementType.UnsignedInteger);

		/// <summary>The track identification for the given Chapter Codec.</summary>
		public static readonly ElementDescriptor TrackTranslate = new ElementDescriptor(0x00006624, "TrackTranslate", ElementType.MasterElement);

		/// <summary>Specify an edition UID on which this translation applies. When not specified, it means for all editions found in the Segment.</summary>
		public static readonly ElementDescriptor TrackTranslateEditionUID = new ElementDescriptor(0x000066FC, "TrackTranslateEditionUID", ElementType.UnsignedInteger);

		/// <summary>The chapter codec using this ID (0: Matroska Script, 1: DVD-menu).</summary>
		public static readonly ElementDescriptor TrackTranslateCodec = new ElementDescriptor(0x000066BF, "TrackTranslateCodec", ElementType.UnsignedInteger);

		/// <summary>The binary value used to represent this track in the chapter codec data. The format depends on the ChapProcessCodecID used.</summary>
		public static readonly ElementDescriptor TrackTranslateTrackID = new ElementDescriptor(0x000066A5, "TrackTranslateTrackID", ElementType.Binary);

		/// <summary>Video settings.</summary>
		public static readonly ElementDescriptor Video = new ElementDescriptor(0x000000E0, "Video", ElementType.MasterElement);

		/// <summary>A flag to declare is the video is known to be progressive or interlaced and if applicable to declare details about the interlacement. (0: undetermined, 1: interlaced, 2: progressive)</summary>
		public static readonly ElementDescriptor FlagInterlaced = new ElementDescriptor(0x0000009A, "FlagInterlaced", ElementType.UnsignedInteger);

		/// <summary>Declare the field ordering of the video. If FlagInterlaced is not set to 1, this Element MUST be ignored. (0: Progressive, 1: Interlaced with top field display first and top field stored first, 2: Undetermined field order, 6: Interlaced with bottom field displayed first and bottom field stored first, 9: Interlaced with bottom field displayed first and top field stored first, 14: Interlaced with top field displayed first and bottom field stored first)</summary>
		public static readonly ElementDescriptor FieldOrder = new ElementDescriptor(0x0000009D, "FieldOrder", ElementType.UnsignedInteger);

		/// <summary>Stereo-3D video mode (0: mono, 1: side by side (left eye is first), 2: top-bottom (right eye is first), 3: top-bottom (left eye is first), 4: checkboard (right is first), 5: checkboard (left is first), 6: row interleaved (right is first), 7: row interleaved (left is first), 8: column interleaved (right is first), 9: column interleaved (left is first), 10: anaglyph (cyan/red), 11: side by side (right eye is first), 12: anaglyph (green/magenta), 13 both eyes laced in one Block (left eye is first), 14 both eyes laced in one Block (right eye is first)) . There are some more details on 3D support in the Specification Notes.</summary>
		public static readonly ElementDescriptor StereoMode = new ElementDescriptor(0x000053B8, "StereoMode", ElementType.UnsignedInteger);

		/// <summary>Alpha Video Mode. Presence of this Element indicates that the BlockAdditional Element could contain Alpha data.</summary>
		public static readonly ElementDescriptor AlphaMode = new ElementDescriptor(0x000053C0, "AlphaMode", ElementType.UnsignedInteger);

		/// <summary>DEPRECATED, DO NOT USE. Bogus StereoMode value used in old versions of libmatroska. (0: mono, 1: right eye, 2: left eye, 3: both eyes).</summary>
		public static readonly ElementDescriptor OldStereoMode = new ElementDescriptor(0x000053B9, "OldStereoMode", ElementType.UnsignedInteger);

		/// <summary>Width of the encoded video frames in pixels.</summary>
		public static readonly ElementDescriptor PixelWidth = new ElementDescriptor(0x000000B0, "PixelWidth", ElementType.UnsignedInteger);

		/// <summary>Height of the encoded video frames in pixels.</summary>
		public static readonly ElementDescriptor PixelHeight = new ElementDescriptor(0x000000BA, "PixelHeight", ElementType.UnsignedInteger);

		/// <summary>The number of video pixels to remove at the bottom of the image (for HDTV content).</summary>
		public static readonly ElementDescriptor PixelCropBottom = new ElementDescriptor(0x000054AA, "PixelCropBottom", ElementType.UnsignedInteger);

		/// <summary>The number of video pixels to remove at the top of the image.</summary>
		public static readonly ElementDescriptor PixelCropTop = new ElementDescriptor(0x000054BB, "PixelCropTop", ElementType.UnsignedInteger);

		/// <summary>The number of video pixels to remove on the left of the image.</summary>
		public static readonly ElementDescriptor PixelCropLeft = new ElementDescriptor(0x000054CC, "PixelCropLeft", ElementType.UnsignedInteger);

		/// <summary>The number of video pixels to remove on the right of the image.</summary>
		public static readonly ElementDescriptor PixelCropRight = new ElementDescriptor(0x000054DD, "PixelCropRight", ElementType.UnsignedInteger);

		/// <summary>Width of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements). The default value is only valid when DisplayUnit is 0.</summary>
		public static readonly ElementDescriptor DisplayWidth = new ElementDescriptor(0x000054B0, "DisplayWidth", ElementType.UnsignedInteger);

		/// <summary>Height of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements). The default value is only valid when DisplayUnit is 0.</summary>
		public static readonly ElementDescriptor DisplayHeight = new ElementDescriptor(0x000054BA, "DisplayHeight", ElementType.UnsignedInteger);

		/// <summary>How DisplayWidth & DisplayHeight should be interpreted (0: pixels, 1: centimeters, 2: inches, 3: Display Aspect Ratio, 4: Unknown).</summary>
		public static readonly ElementDescriptor DisplayUnit = new ElementDescriptor(0x000054B2, "DisplayUnit", ElementType.UnsignedInteger);

		/// <summary>Specify the possible modifications to the aspect ratio (0: free resizing, 1: keep aspect ratio, 2: fixed).</summary>
		public static readonly ElementDescriptor AspectRatioType = new ElementDescriptor(0x000054B3, "AspectRatioType", ElementType.UnsignedInteger);

		/// <summary>Same value as in AVI (32 bits).</summary>
		public static readonly ElementDescriptor ColourSpace = new ElementDescriptor(0x002EB524, "ColourSpace", ElementType.Binary);

		/// <summary>Gamma Value.</summary>
		public static readonly ElementDescriptor GammaValue = new ElementDescriptor(0x002FB523, "GammaValue", ElementType.Float);

		/// <summary>Number of frames per second. Informational only.</summary>
		public static readonly ElementDescriptor FrameRate = new ElementDescriptor(0x002383E3, "FrameRate", ElementType.Float);

		/// <summary> Settings describing the colour format.</summary>
		public static readonly ElementDescriptor Colour = new ElementDescriptor(0x000055B0, "Colour", ElementType.MasterElement);

		/// <summary>The Matrix Coefficients of the video used to derive luma and chroma values from reg, green, and blue color primaries. For clarity, the value and meanings for MatrixCoefficients are adopted from Table 4 of ISO/IEC 23001-8:2013/DCOR1. (0:GBR, 1: BT709, 2: Unspecified, 3: Reserved, 4: FCC, 5: BT470BG, 6: SMPTE 170M, 7: SMPTE 240M, 8: YCOCG, 9: BT2020 Non-constant Luminance, 10: BT2020 Constant Luminance)</summary>
		public static readonly ElementDescriptor MatrixCoefficients = new ElementDescriptor(0x000055B1, "MatrixCoefficients", ElementType.UnsignedInteger);

		/// <summary>Number of decoded bits per channel. A value of 0 indicates that the BitsPerChannel is unspecified.</summary>
		public static readonly ElementDescriptor BitsPerChannel = new ElementDescriptor(0x000055B2, "BitsPerChannel", ElementType.UnsignedInteger);

		/// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed horizontally. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingHorz should be set to 1.</summary>
		public static readonly ElementDescriptor ChromaSubsamplingHorz = new ElementDescriptor(0x000055B3, "ChromaSubsamplingHorz", ElementType.UnsignedInteger);

		/// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed vertically. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingVert should be set to 1.</summary>
		public static readonly ElementDescriptor ChromaSubsamplingVert = new ElementDescriptor(0x000055B4, "ChromaSubsamplingVert", ElementType.UnsignedInteger);

		/// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed horizontally. This is additive with ChromaSubsamplingHorz. Example: For video with 4:2:1 chroma subsampling, the ChromaSubsamplingHorz should be set to 1 and CbSubsamplingHorz should be set to 1.</summary>
		public static readonly ElementDescriptor CbSubsamplingHorz = new ElementDescriptor(0x000055B5, "CbSubsamplingHorz", ElementType.UnsignedInteger);

		/// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed vertically. This is additive with ChromaSubsamplingVert.</summary>
		public static readonly ElementDescriptor CbSubsamplingVert = new ElementDescriptor(0x000055B6, "CbSubsamplingVert", ElementType.UnsignedInteger);

		/// <summary>How chroma is subsampled horizontally. (0: Unspecified, 1: Left Collocated, 2: Half)</summary>
		public static readonly ElementDescriptor ChromaSitingHorz = new ElementDescriptor(0x000055B7, "ChromaSitingHorz", ElementType.UnsignedInteger);

		/// <summary>How chroma is subsampled vertically. (0: Unspecified, 1: Top Collocated, 2: Half)</summary>
		public static readonly ElementDescriptor ChromaSitingVert = new ElementDescriptor(0x000055B8, "ChromaSitingVert", ElementType.UnsignedInteger);

		/// <summary>Clipping of the color ranges. (0: Unspecified, 1: Broadcast Range, 2: Full range (no clipping), 3: Defined by MatrixCoefficients/TransferCharacteristics)</summary>
		public static readonly ElementDescriptor Range = new ElementDescriptor(0x000055B9, "Range", ElementType.UnsignedInteger);

		/// <summary>The transfer characteristics of the video. For clarity, the value and meanings for TransferCharacteristics 1-15 are adopted from Table 3 of ISO/IEC 23001-8:2013/DCOR1. TransferCharacteristics 16-18 are proposed values. (0: Reserved, 1: ITU-R BT.709, 2: Unspecified, 3: Reserved, 4: Gamma 2.2 curve, 5: Gamma 2.8 curve, 6: SMPTE 170M, 7: SMPTE 240M, 8: Linear, 9: Log, 10: Log Sqrt, 11: IEC 61966-2-4, 12: ITU-R BT.1361 Extended Colour Gamut, 13: IEC 61966-2-1, 14: ITU-R BT.2020 10 bit, 15: ITU-R BT.2020 12 bit, 16: SMPTE ST 2084, 17: SMPTE ST 428-1 18: ARIB STD-B67 (HLG))</summary>
		public static readonly ElementDescriptor TransferCharacteristics = new ElementDescriptor(0x000055BA, "TransferCharacteristics", ElementType.UnsignedInteger);

		/// <summary>The colour primaries of the video. For clarity, the value and meanings for Primaries are adopted from Table 2 of ISO/IEC 23001-8:2013/DCOR1. (0: Reserved, 1: ITU-R BT.709, 2: Unspecified, 3: Reserved, 4: ITU-R BT.470M, 5: ITU-R BT.470BG, 6: SMPTE 170M, 7: SMPTE 240M, 8: FILM, 9: ITU-R BT.2020, 10: SMPTE ST 428-1, 22: JEDEC P22 phosphors)</summary>
		public static readonly ElementDescriptor Primaries = new ElementDescriptor(0x000055BB, "Primaries", ElementType.UnsignedInteger);

		/// <summary>Maximum brightness of a single pixel (Maximum Content Light Level) in candelas per square meter (cd/mý).</summary>
		public static readonly ElementDescriptor MaxCLL = new ElementDescriptor(0x000055BC, "MaxCLL", ElementType.UnsignedInteger);

		/// <summary>Maximum brightness of a single full frame (Maximum Frame-Average Light Level) in candelas per square meter (cd/mý).</summary>
		public static readonly ElementDescriptor MaxFALL = new ElementDescriptor(0x000055BD, "MaxFALL", ElementType.UnsignedInteger);

		/// <summary>SMPTE 2086 mastering data.</summary>
		public static readonly ElementDescriptor MasteringMetadata = new ElementDescriptor(0x000055D0, "MasteringMetadata", ElementType.MasterElement);

		/// <summary>Red X chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly ElementDescriptor PrimaryRChromaticityX = new ElementDescriptor(0x000055D1, "PrimaryRChromaticityX", ElementType.Float);

		/// <summary>Red Y chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly ElementDescriptor PrimaryRChromaticityY = new ElementDescriptor(0x000055D2, "PrimaryRChromaticityY", ElementType.Float);

		/// <summary>Green X chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly ElementDescriptor PrimaryGChromaticityX = new ElementDescriptor(0x000055D3, "PrimaryGChromaticityX", ElementType.Float);

		/// <summary>Green Y chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly ElementDescriptor PrimaryGChromaticityY = new ElementDescriptor(0x000055D4, "PrimaryGChromaticityY", ElementType.Float);

		/// <summary>Blue X chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly ElementDescriptor PrimaryBChromaticityX = new ElementDescriptor(0x000055D5, "PrimaryBChromaticityX", ElementType.Float);

		/// <summary>Blue Y chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly ElementDescriptor PrimaryBChromaticityY = new ElementDescriptor(0x000055D6, "PrimaryBChromaticityY", ElementType.Float);

		/// <summary>White X chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly ElementDescriptor WhitePointChromaticityX = new ElementDescriptor(0x000055D7, "WhitePointChromaticityX", ElementType.Float);

		/// <summary>White Y chromaticity coordinate as defined by CIE 1931.</summary>
		public static readonly ElementDescriptor WhitePointChromaticityY = new ElementDescriptor(0x000055D8, "WhitePointChromaticityY", ElementType.Float);

		/// <summary>Maximum luminance. Shall be represented in candelas per square meter (cd/mý).</summary>
		public static readonly ElementDescriptor LuminanceMax = new ElementDescriptor(0x000055D9, "LuminanceMax", ElementType.Float);

		/// <summary>Mininum luminance. Shall be represented in candelas per square meter (cd/mý).</summary>
		public static readonly ElementDescriptor LuminanceMin = new ElementDescriptor(0x000055DA, "LuminanceMin", ElementType.Float);

		/// <summary>Audio settings.</summary>
		public static readonly ElementDescriptor Audio = new ElementDescriptor(0x000000E1, "Audio", ElementType.MasterElement);

		/// <summary>Sampling frequency in Hz.</summary>
		public static readonly ElementDescriptor SamplingFrequency = new ElementDescriptor(0x000000B5, "SamplingFrequency", ElementType.Float);

		/// <summary>Real output sampling frequency in Hz (used for SBR techniques).</summary>
		public static readonly ElementDescriptor OutputSamplingFrequency = new ElementDescriptor(0x000078B5, "OutputSamplingFrequency", ElementType.Float);

		/// <summary>Numbers of channels in the track.</summary>
		public static readonly ElementDescriptor Channels = new ElementDescriptor(0x0000009F, "Channels", ElementType.UnsignedInteger);

		/// <summary>Table of horizontal angles for each successive channel, see appendix.</summary>
		public static readonly ElementDescriptor ChannelPositions = new ElementDescriptor(0x00007D7B, "ChannelPositions", ElementType.Binary);

		/// <summary>Bits per sample, mostly used for PCM.</summary>
		public static readonly ElementDescriptor BitDepth = new ElementDescriptor(0x00006264, "BitDepth", ElementType.UnsignedInteger);

		/// <summary>Operation that needs to be applied on tracks to create this virtual track. For more details look at the Specification Notes on the subject.</summary>
		public static readonly ElementDescriptor TrackOperation = new ElementDescriptor(0x000000E2, "TrackOperation", ElementType.MasterElement);

		/// <summary>Contains the list of all video plane tracks that need to be combined to create this 3D track</summary>
		public static readonly ElementDescriptor TrackCombinePlanes = new ElementDescriptor(0x000000E3, "TrackCombinePlanes", ElementType.MasterElement);

		/// <summary>Contains a video plane track that need to be combined to create this 3D track</summary>
		public static readonly ElementDescriptor TrackPlane = new ElementDescriptor(0x000000E4, "TrackPlane", ElementType.MasterElement);

		/// <summary>The trackUID number of the track representing the plane.</summary>
		public static readonly ElementDescriptor TrackPlaneUID = new ElementDescriptor(0x000000E5, "TrackPlaneUID", ElementType.UnsignedInteger);

		/// <summary>The kind of plane this track corresponds to (0: left eye, 1: right eye, 2: background).</summary>
		public static readonly ElementDescriptor TrackPlaneType = new ElementDescriptor(0x000000E6, "TrackPlaneType", ElementType.UnsignedInteger);

		/// <summary>Contains the list of all tracks whose Blocks need to be combined to create this virtual track</summary>
		public static readonly ElementDescriptor TrackJoinBlocks = new ElementDescriptor(0x000000E9, "TrackJoinBlocks", ElementType.MasterElement);

		/// <summary>The trackUID number of a track whose blocks are used to create this virtual track.</summary>
		public static readonly ElementDescriptor TrackJoinUID = new ElementDescriptor(0x000000ED, "TrackJoinUID", ElementType.UnsignedInteger);

		/// <summary>DivX trick track extenstions</summary>
		public static readonly ElementDescriptor TrickTrackUID = new ElementDescriptor(0x000000C0, "TrickTrackUID", ElementType.UnsignedInteger);

		/// <summary>DivX trick track extenstions</summary>
		public static readonly ElementDescriptor TrickTrackSegmentUID = new ElementDescriptor(0x000000C1, "TrickTrackSegmentUID", ElementType.Binary);

		/// <summary>DivX trick track extenstions</summary>
		public static readonly ElementDescriptor TrickTrackFlag = new ElementDescriptor(0x000000C6, "TrickTrackFlag", ElementType.UnsignedInteger);

		/// <summary>DivX trick track extenstions</summary>
		public static readonly ElementDescriptor TrickMasterTrackUID = new ElementDescriptor(0x000000C7, "TrickMasterTrackUID", ElementType.UnsignedInteger);

		/// <summary>DivX trick track extenstions</summary>
		public static readonly ElementDescriptor TrickMasterTrackSegmentUID = new ElementDescriptor(0x000000C4, "TrickMasterTrackSegmentUID", ElementType.Binary);

		/// <summary>Settings for several content encoding mechanisms like compression or encryption.</summary>
		public static readonly ElementDescriptor ContentEncodings = new ElementDescriptor(0x00006D80, "ContentEncodings", ElementType.MasterElement);

		/// <summary>Settings for one content encoding like compression or encryption.</summary>
		public static readonly ElementDescriptor ContentEncoding = new ElementDescriptor(0x00006240, "ContentEncoding", ElementType.MasterElement);

		/// <summary>Tells when this modification was used during encoding/muxing starting with 0 and counting upwards. The decoder/demuxer has to start with the highest order number it finds and work its way down. This value has to be unique over all ContentEncodingOrder Elements in the Segment.</summary>
		public static readonly ElementDescriptor ContentEncodingOrder = new ElementDescriptor(0x00005031, "ContentEncodingOrder", ElementType.UnsignedInteger);

		/// <summary>A bit field that describes which Elements have been modified in this way. Values (big endian) can be OR'ed. Possible values: 1 - all frame contents, 2 - the track's private data, 4 - the next ContentEncoding (next ContentEncodingOrder. Either the data inside ContentCompression and/or ContentEncryption)</summary>
		public static readonly ElementDescriptor ContentEncodingScope = new ElementDescriptor(0x00005032, "ContentEncodingScope", ElementType.UnsignedInteger);

		/// <summary>A value describing what kind of transformation has been done. Possible values: 0 - compression, 1 - encryption</summary>
		public static readonly ElementDescriptor ContentEncodingType = new ElementDescriptor(0x00005033, "ContentEncodingType", ElementType.UnsignedInteger);

		/// <summary>Settings describing the compression used. Must be present if the value of ContentEncodingType is 0 and absent otherwise. Each block must be decompressable even if no previous block is available in order not to prevent seeking.</summary>
		public static readonly ElementDescriptor ContentCompression = new ElementDescriptor(0x00005034, "ContentCompression", ElementType.MasterElement);

		/// <summary>The compression algorithm used. Algorithms that have been specified so far are: 0 - zlib,1 - bzlib,2 - lzo1x 3 - Header Stripping</summary>
		public static readonly ElementDescriptor ContentCompAlgo = new ElementDescriptor(0x00004254, "ContentCompAlgo", ElementType.UnsignedInteger);

		/// <summary>Settings that might be needed by the decompressor. For Header Stripping (ContentCompAlgo=3), the bytes that were removed from the beggining of each frames of the track.</summary>
		public static readonly ElementDescriptor ContentCompSettings = new ElementDescriptor(0x00004255, "ContentCompSettings", ElementType.Binary);

		/// <summary>Settings describing the encryption used. Must be present if the value of ContentEncodingType is 1 and absent otherwise.</summary>
		public static readonly ElementDescriptor ContentEncryption = new ElementDescriptor(0x00005035, "ContentEncryption", ElementType.MasterElement);

		/// <summary>The encryption algorithm used. The value '0' means that the contents have not been encrypted but only signed. Predefined values: 1 - DES, 2 - 3DES, 3 - Twofish, 4 - Blowfish, 5 - AES</summary>
		public static readonly ElementDescriptor ContentEncAlgo = new ElementDescriptor(0x000047E1, "ContentEncAlgo", ElementType.UnsignedInteger);

		/// <summary>For public key algorithms this is the ID of the public key the the data was encrypted with.</summary>
		public static readonly ElementDescriptor ContentEncKeyID = new ElementDescriptor(0x000047E2, "ContentEncKeyID", ElementType.Binary);

		/// <summary>A cryptographic signature of the contents.</summary>
		public static readonly ElementDescriptor ContentSignature = new ElementDescriptor(0x000047E3, "ContentSignature", ElementType.Binary);

		/// <summary>This is the ID of the private key the data was signed with.</summary>
		public static readonly ElementDescriptor ContentSigKeyID = new ElementDescriptor(0x000047E4, "ContentSigKeyID", ElementType.Binary);

		/// <summary>The algorithm used for the signature. A value of '0' means that the contents have not been signed but only encrypted. Predefined values: 1 - RSA</summary>
		public static readonly ElementDescriptor ContentSigAlgo = new ElementDescriptor(0x000047E5, "ContentSigAlgo", ElementType.UnsignedInteger);

		/// <summary>The hash algorithm used for the signature. A value of '0' means that the contents have not been signed but only encrypted. Predefined values: 1 - SHA1-160 2 - MD5</summary>
		public static readonly ElementDescriptor ContentSigHashAlgo = new ElementDescriptor(0x000047E6, "ContentSigHashAlgo", ElementType.UnsignedInteger);

		/// <summary>A Top-Level Element to speed seeking access. All entries are local to the Segment. Should be mandatory for non "live" streams.</summary>
		public static readonly ElementDescriptor Cues = new ElementDescriptor(0x1C53BB6B, "Cues", ElementType.MasterElement);

		/// <summary>Contains all information relative to a seek point in the Segment.</summary>
		public static readonly ElementDescriptor CuePoint = new ElementDescriptor(0x000000BB, "CuePoint", ElementType.MasterElement);

		/// <summary>Absolute timestamp according to the Segment time base.</summary>
		public static readonly ElementDescriptor CueTime = new ElementDescriptor(0x000000B3, "CueTime", ElementType.UnsignedInteger);

		/// <summary>Contain positions for different tracks corresponding to the timestamp.</summary>
		public static readonly ElementDescriptor CueTrackPositions = new ElementDescriptor(0x000000B7, "CueTrackPositions", ElementType.MasterElement);

		/// <summary>The track for which a position is given.</summary>
		public static readonly ElementDescriptor CueTrack = new ElementDescriptor(0x000000F7, "CueTrack", ElementType.UnsignedInteger);

		/// <summary>The position of the Cluster containing the required Block.</summary>
		public static readonly ElementDescriptor CueClusterPosition = new ElementDescriptor(0x000000F1, "CueClusterPosition", ElementType.UnsignedInteger);

		/// <summary>The relative position of the referenced block inside the cluster with 0 being the first possible position for an Element inside that cluster.</summary>
		public static readonly ElementDescriptor CueRelativePosition = new ElementDescriptor(0x000000F0, "CueRelativePosition", ElementType.UnsignedInteger);

		/// <summary>The duration of the block according to the Segment time base. If missing the track's DefaultDuration does not apply and no duration information is available in terms of the cues.</summary>
		public static readonly ElementDescriptor CueDuration = new ElementDescriptor(0x000000B2, "CueDuration", ElementType.UnsignedInteger);

		/// <summary>Number of the Block in the specified Cluster.</summary>
		public static readonly ElementDescriptor CueBlockNumber = new ElementDescriptor(0x00005378, "CueBlockNumber", ElementType.UnsignedInteger);

		/// <summary>The position of the Codec State corresponding to this Cue Element. 0 means that the data is taken from the initial Track Entry.</summary>
		public static readonly ElementDescriptor CueCodecState = new ElementDescriptor(0x000000EA, "CueCodecState", ElementType.UnsignedInteger);

		/// <summary>The Clusters containing the required referenced Blocks.</summary>
		public static readonly ElementDescriptor CueReference = new ElementDescriptor(0x000000DB, "CueReference", ElementType.MasterElement);

		/// <summary>Timestamp of the referenced Block.</summary>
		public static readonly ElementDescriptor CueRefTime = new ElementDescriptor(0x00000096, "CueRefTime", ElementType.UnsignedInteger);

		/// <summary>The Position of the Cluster containing the referenced Block.</summary>
		public static readonly ElementDescriptor CueRefCluster = new ElementDescriptor(0x00000097, "CueRefCluster", ElementType.UnsignedInteger);

		/// <summary>Number of the referenced Block of Track X in the specified Cluster.</summary>
		public static readonly ElementDescriptor CueRefNumber = new ElementDescriptor(0x0000535F, "CueRefNumber", ElementType.UnsignedInteger);

		/// <summary>The position of the Codec State corresponding to this referenced Element. 0 means that the data is taken from the initial Track Entry.</summary>
		public static readonly ElementDescriptor CueRefCodecState = new ElementDescriptor(0x000000EB, "CueRefCodecState", ElementType.UnsignedInteger);

		/// <summary>Contain attached files.</summary>
		public static readonly ElementDescriptor Attachments = new ElementDescriptor(0x1941A469, "Attachments", ElementType.MasterElement);

		/// <summary>An attached file.</summary>
		public static readonly ElementDescriptor AttachedFile = new ElementDescriptor(0x000061A7, "AttachedFile", ElementType.MasterElement);

		/// <summary>A human-friendly name for the attached file.</summary>
		public static readonly ElementDescriptor FileDescription = new ElementDescriptor(0x0000467E, "FileDescription", ElementType.Utf8String);

		/// <summary>Filename of the attached file.</summary>
		public static readonly ElementDescriptor FileName = new ElementDescriptor(0x0000466E, "FileName", ElementType.Utf8String);

		/// <summary>MIME type of the file.</summary>
		public static readonly ElementDescriptor FileMimeType = new ElementDescriptor(0x00004660, "FileMimeType", ElementType.AsciiString);

		/// <summary>The data of the file.</summary>
		public static readonly ElementDescriptor FileData = new ElementDescriptor(0x0000465C, "FileData", ElementType.Binary);

		/// <summary>Unique ID representing the file, as random as possible.</summary>
		public static readonly ElementDescriptor FileUID = new ElementDescriptor(0x000046AE, "FileUID", ElementType.UnsignedInteger);

		/// <summary>A binary value that a track/codec can refer to when the attachment is needed.</summary>
		public static readonly ElementDescriptor FileReferral = new ElementDescriptor(0x00004675, "FileReferral", ElementType.Binary);

		/// <summary>DivX font extension</summary>
		public static readonly ElementDescriptor FileUsedStartTime = new ElementDescriptor(0x00004661, "FileUsedStartTime", ElementType.UnsignedInteger);

		/// <summary>DivX font extension</summary>
		public static readonly ElementDescriptor FileUsedEndTime = new ElementDescriptor(0x00004662, "FileUsedEndTime", ElementType.UnsignedInteger);

		/// <summary>A system to define basic menus and partition data. For more detailed information, look at the Chapters Explanation.</summary>
		public static readonly ElementDescriptor Chapters = new ElementDescriptor(0x1043A770, "Chapters", ElementType.MasterElement);

		/// <summary>Contains all information about a Segment edition.</summary>
		public static readonly ElementDescriptor EditionEntry = new ElementDescriptor(0x000045B9, "EditionEntry", ElementType.MasterElement);

		/// <summary>A unique ID to identify the edition. It's useful for tagging an edition.</summary>
		public static readonly ElementDescriptor EditionUID = new ElementDescriptor(0x000045BC, "EditionUID", ElementType.UnsignedInteger);

		/// <summary>If an edition is hidden (1), it should not be available to the user interface (but still to Control Tracks; see flag notes). (1 bit)</summary>
		public static readonly ElementDescriptor EditionFlagHidden = new ElementDescriptor(0x000045BD, "EditionFlagHidden", ElementType.UnsignedInteger);

		/// <summary>If a flag is set (1) the edition should be used as the default one. (1 bit)</summary>
		public static readonly ElementDescriptor EditionFlagDefault = new ElementDescriptor(0x000045DB, "EditionFlagDefault", ElementType.UnsignedInteger);

		/// <summary>Specify if the chapters can be defined multiple times and the order to play them is enforced. (1 bit)</summary>
		public static readonly ElementDescriptor EditionFlagOrdered = new ElementDescriptor(0x000045DD, "EditionFlagOrdered", ElementType.UnsignedInteger);

		/// <summary>Contains the atom information to use as the chapter atom (apply to all tracks).</summary>
		public static readonly ElementDescriptor ChapterAtom = new ElementDescriptor(0x000000B6, "ChapterAtom", ElementType.MasterElement);

		/// <summary>A unique ID to identify the Chapter.</summary>
		public static readonly ElementDescriptor ChapterUID = new ElementDescriptor(0x000073C4, "ChapterUID", ElementType.UnsignedInteger);

		/// <summary>A unique string ID to identify the Chapter. Use for WebVTT cue identifier storage.</summary>
		public static readonly ElementDescriptor ChapterStringUID = new ElementDescriptor(0x00005654, "ChapterStringUID", ElementType.Utf8String);

		/// <summary>Timestamp of the start of Chapter (not scaled).</summary>
		public static readonly ElementDescriptor ChapterTimeStart = new ElementDescriptor(0x00000091, "ChapterTimeStart", ElementType.UnsignedInteger);

		/// <summary>Timestamp of the end of Chapter (timestamp excluded, not scaled).</summary>
		public static readonly ElementDescriptor ChapterTimeEnd = new ElementDescriptor(0x00000092, "ChapterTimeEnd", ElementType.UnsignedInteger);

		/// <summary>If a chapter is hidden (1), it should not be available to the user interface (but still to Control Tracks; see flag notes). (1 bit)</summary>
		public static readonly ElementDescriptor ChapterFlagHidden = new ElementDescriptor(0x00000098, "ChapterFlagHidden", ElementType.UnsignedInteger);

		/// <summary>Specify wether the chapter is enabled. It can be enabled/disabled by a Control Track. When disabled, the movie should skip all the content between the TimeStart and TimeEnd of this chapter (see flag notes). (1 bit)</summary>
		public static readonly ElementDescriptor ChapterFlagEnabled = new ElementDescriptor(0x00004598, "ChapterFlagEnabled", ElementType.UnsignedInteger);

		/// <summary>A Segment to play in place of this chapter. Edition ChapterSegmentEditionUID should be used for this Segment, otherwise no edition is used.</summary>
		public static readonly ElementDescriptor ChapterSegmentUID = new ElementDescriptor(0x00006E67, "ChapterSegmentUID", ElementType.Binary);

		/// <summary>The EditionUID to play from the Segment linked in ChapterSegmentUID.</summary>
		public static readonly ElementDescriptor ChapterSegmentEditionUID = new ElementDescriptor(0x00006EBC, "ChapterSegmentEditionUID", ElementType.UnsignedInteger);

		/// <summary>Specify the physical equivalent of this ChapterAtom like "DVD" (60) or "SIDE" (50), see complete list of values.</summary>
		public static readonly ElementDescriptor ChapterPhysicalEquiv = new ElementDescriptor(0x000063C3, "ChapterPhysicalEquiv", ElementType.UnsignedInteger);

		/// <summary>List of tracks on which the chapter applies. If this Element is not present, all tracks apply</summary>
		public static readonly ElementDescriptor ChapterTrack = new ElementDescriptor(0x0000008F, "ChapterTrack", ElementType.MasterElement);

		/// <summary>UID of the Track to apply this chapter too. In the absence of a control track, choosing this chapter will select the listed Tracks and deselect unlisted tracks. Absence of this Element indicates that the Chapter should be applied to any currently used Tracks.</summary>
		public static readonly ElementDescriptor ChapterTrackNumber = new ElementDescriptor(0x00000089, "ChapterTrackNumber", ElementType.UnsignedInteger);

		/// <summary>Contains all possible strings to use for the chapter display.</summary>
		public static readonly ElementDescriptor ChapterDisplay = new ElementDescriptor(0x00000080, "ChapterDisplay", ElementType.MasterElement);

		/// <summary>Contains the string to use as the chapter atom.</summary>
		public static readonly ElementDescriptor ChapString = new ElementDescriptor(0x00000085, "ChapString", ElementType.Utf8String);

		/// <summary>The languages corresponding to the string, in the bibliographic ISO-639-2 form.</summary>
		public static readonly ElementDescriptor ChapLanguage = new ElementDescriptor(0x0000437C, "ChapLanguage", ElementType.AsciiString);

		/// <summary>The countries corresponding to the string, same 2 octets as in Internet domains.</summary>
		public static readonly ElementDescriptor ChapCountry = new ElementDescriptor(0x0000437E, "ChapCountry", ElementType.AsciiString);

		/// <summary>Contains all the commands associated to the Atom.</summary>
		public static readonly ElementDescriptor ChapProcess = new ElementDescriptor(0x00006944, "ChapProcess", ElementType.MasterElement);

		/// <summary>Contains the type of the codec used for the processing. A value of 0 means native Matroska processing (to be defined), a value of 1 means the DVD command set is used. More codec IDs can be added later.</summary>
		public static readonly ElementDescriptor ChapProcessCodecID = new ElementDescriptor(0x00006955, "ChapProcessCodecID", ElementType.UnsignedInteger);

		/// <summary>Some optional data attached to the ChapProcessCodecID information. For ChapProcessCodecID = 1, it is the "DVD level" equivalent.</summary>
		public static readonly ElementDescriptor ChapProcessPrivate = new ElementDescriptor(0x0000450D, "ChapProcessPrivate", ElementType.Binary);

		/// <summary>Contains all the commands associated to the Atom.</summary>
		public static readonly ElementDescriptor ChapProcessCommand = new ElementDescriptor(0x00006911, "ChapProcessCommand", ElementType.MasterElement);

		/// <summary>Defines when the process command should be handled (0: during the whole chapter, 1: before starting playback, 2: after playback of the chapter).</summary>
		public static readonly ElementDescriptor ChapProcessTime = new ElementDescriptor(0x00006922, "ChapProcessTime", ElementType.UnsignedInteger);

		/// <summary>Contains the command information. The data should be interpreted depending on the ChapProcessCodecID value. For ChapProcessCodecID = 1, the data correspond to the binary DVD cell pre/post commands.</summary>
		public static readonly ElementDescriptor ChapProcessData = new ElementDescriptor(0x00006933, "ChapProcessData", ElementType.Binary);

		/// <summary>Element containing Elements specific to Tracks/Chapters. A list of valid tags can be found here.</summary>
		public static readonly ElementDescriptor Tags = new ElementDescriptor(0x1254C367, "Tags", ElementType.MasterElement);

		/// <summary>Element containing Elements specific to Tracks/Chapters.</summary>
		public static readonly ElementDescriptor Tag = new ElementDescriptor(0x00007373, "Tag", ElementType.MasterElement);

		/// <summary>Contain all UIDs where the specified meta data apply. It is empty to describe everything in the Segment.</summary>
		public static readonly ElementDescriptor Targets = new ElementDescriptor(0x000063C0, "Targets", ElementType.MasterElement);

		/// <summary>A number to indicate the logical level of the target (see TargetType).</summary>
		public static readonly ElementDescriptor TargetTypeValue = new ElementDescriptor(0x000068CA, "TargetTypeValue", ElementType.UnsignedInteger);

		/// <summary>An informational string that can be used to display the logical level of the target like "ALBUM", "TRACK", "MOVIE", "CHAPTER", etc (see TargetType).</summary>
		public static readonly ElementDescriptor TargetType = new ElementDescriptor(0x000063CA, "TargetType", ElementType.AsciiString);

		/// <summary>A unique ID to identify the Track(s) the tags belong to. If the value is 0 at this level, the tags apply to all tracks in the Segment.</summary>
		public static readonly ElementDescriptor TagTrackUID = new ElementDescriptor(0x000063C5, "TagTrackUID", ElementType.UnsignedInteger);

		/// <summary>A unique ID to identify the EditionEntry(s) the tags belong to. If the value is 0 at this level, the tags apply to all editions in the Segment.</summary>
		public static readonly ElementDescriptor TagEditionUID = new ElementDescriptor(0x000063C9, "TagEditionUID", ElementType.UnsignedInteger);

		/// <summary>A unique ID to identify the Chapter(s) the tags belong to. If the value is 0 at this level, the tags apply to all chapters in the Segment.</summary>
		public static readonly ElementDescriptor TagChapterUID = new ElementDescriptor(0x000063C4, "TagChapterUID", ElementType.UnsignedInteger);

		/// <summary>A unique ID to identify the Attachment(s) the tags belong to. If the value is 0 at this level, the tags apply to all the attachments in the Segment.</summary>
		public static readonly ElementDescriptor TagAttachmentUID = new ElementDescriptor(0x000063C6, "TagAttachmentUID", ElementType.UnsignedInteger);

		/// <summary>Contains general information about the target.</summary>
		public static readonly ElementDescriptor SimpleTag = new ElementDescriptor(0x000067C8, "SimpleTag", ElementType.MasterElement);

		/// <summary>The name of the Tag that is going to be stored.</summary>
		public static readonly ElementDescriptor TagName = new ElementDescriptor(0x000045A3, "TagName", ElementType.Utf8String);

		/// <summary>Specifies the language of the tag specified, in the Matroska languages form.</summary>
		public static readonly ElementDescriptor TagLanguage = new ElementDescriptor(0x0000447A, "TagLanguage", ElementType.AsciiString);

		/// <summary>Indication to know if this is the default/original language to use for the given tag. (1 bit)</summary>
		public static readonly ElementDescriptor TagDefault = new ElementDescriptor(0x00004484, "TagDefault", ElementType.UnsignedInteger);

		/// <summary>The value of the Tag.</summary>
		public static readonly ElementDescriptor TagString = new ElementDescriptor(0x00004487, "TagString", ElementType.Utf8String);

		/// <summary>The values of the Tag if it is binary. Note that this cannot be used in the same SimpleTag as TagString.</summary>
		public static readonly ElementDescriptor TagBinary = new ElementDescriptor(0x00004485, "TagBinary", ElementType.Binary);

		#endregion
	}
}
