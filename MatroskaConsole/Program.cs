using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using ATL;
using Commons;
using Matroska.Spec;
using NEbml.Core;
using NEbml.MkvTitleEdit.Matroska;
using OggVorbisEncoder;

namespace Matroska
{
    public static class EbmlElementEx
    {
        //public static string GetName(this EbmlElement e)
        //{
        //    string name = MatroskaSpecificationEbml.Elements.ContainsKey(e.ID) ? MatroskaSpecificationEbml.Elements[e.ID].Name : "?";

        //    return $"0x{e.ID:X8} {name} {e.Length} bytes";
        //}

        public static string GetName(this NEbml.Core.EbmlReader r, bool dumpValue = false)
        {
            string name = "?";
            string dump = "?";

            var el = MatroskaSpecification.ElementDescriptors.GetValueOrDefault(r.ElementId);
            if (el != null && el != default)
            {
                name = el.Name;

                if (dumpValue)
                {
                    switch (el.Type)
                    {
                        case ElementType.AsciiString:
                            dump = r.ReadAscii();
                            break;
                        case ElementType.Binary:
                            dump = "binary data";
                            break;
                        case ElementType.Date:
                            dump = r.ReadDate().ToString();
                            break;
                        case ElementType.Float:
                            dump = r.ReadFloat().ToString();
                            break;
                        case ElementType.SignedInteger:
                            dump = r.ReadInt().ToString();
                            break;
                        case ElementType.UnsignedInteger:
                            dump = r.ReadUInt().ToString();
                            break;
                        case ElementType.Utf8String:
                            dump = r.ReadUtf();
                            break;
                        case ElementType.MasterElement:
                            dump = "'Master'";
                            break;
                        default:
                            dump = $"unknown (id:{r.ToString()})";
                            break;
                    }
                }
            }

            return $"0x{r.ElementId.Value:X8} {name} [{r.ElementSize} bytes]" + (dumpValue ? " Value: " + dump : "");
        }
    }

    class Document
    {
        public EbmlHeader EBMLHeader { get; set; }

        public Segment Segment { get; set; }

        public Info Info { get; set; }
    }

    class EbmlHeader
    {

    }

    

    public static class DictionaryEx
    {
        public static async Task InvokeAsync<T>(this IDictionary<uint, Func<T, EbmlElement, Task>> mapping, T subject, EbmlElement element)
        {
            if (mapping.ContainsKey(element.ID))
            {
                await mapping[element.ID].Invoke(subject, element);
            }
        }

        public static void Invoke<T>(this IDictionary<ElementDescriptor, Action<T, NEbml.Core.EbmlReader>> mappings, T subject, NEbml.Core.EbmlReader reader)
        {
            var mapping = mappings.FirstOrDefault(m => m.Key.Identifier == reader.ElementId);

            if (mapping.Key != default)
            {
                mapping.Value.Invoke(subject, reader);
            }
        }
    }


    class MatroskaParser
    {
        public async Task<EbmlHeader> ParseAsync(Stream stream)
        {
            var header = new EbmlHeader();


            return header;
        }
    }

    class OpusHeader
    {
        public String ID;
        public byte Version;
        public byte OutputChannelCount;
        public UInt16 PreSkip;
        public UInt32 InputSampleRate;
        public Int16 OutputGain;
        public byte ChannelMappingFamily;

        public byte StreamCount;
        public byte CoupledStreamCount;
        public byte[] ChannelMapping;

        public void Reset()
        {
            ID = "";
            Version = 0;
            OutputChannelCount = 0;
            PreSkip = 0;
            InputSampleRate = 0;
            OutputGain = 0;
            ChannelMappingFamily = 0;
            StreamCount = 0;
            CoupledStreamCount = 0;
        }
    }

    class OggHeader
    {
        // Ogg page header ID
        const String OGG_PAGE_ID = "OggS";

        // Vorbis identification packet (frame) ID
        static readonly String VORBIS_HEADER_ID = (char)1 + "vorbis";

        // Vorbis tag packet (frame) ID
        static readonly String VORBIS_TAG_ID = (char)3 + "vorbis";

        // Vorbis setup packet (frame) ID
        static readonly String VORBIS_SETUP_ID = (char)5 + "vorbis";

        // Vorbis parameter frame ID
        const String OPUS_HEADER_ID = "OpusHead";

        // Opus tag frame ID
        const String OPUS_TAG_ID = "OpusTags";


        public string ID;                                               // Always "OggS"
        public byte StreamVersion;                           // Stream structure version
        public byte TypeFlag;                                        // Header type flag
        public ulong AbsolutePosition;                      // Absolute granule position
        public int Serial;                                       // Stream serial number
        public int PageNumber;                                   // Page sequence number
        public uint Checksum;                                              // Page CRC32
        public byte Segments;                                 // Number of page segments
        public byte[] LacingValues;                     // Lacing values - segment sizes

        public void Reset()
        {
            ID = "";
            StreamVersion = 0;
            TypeFlag = 0;
            AbsolutePosition = 0;
            Serial = 0;
            PageNumber = 0;
            Checksum = 0;
            Segments = 0;
        }

        public void ReadFromStream(BufferedBinaryReader r)
        {
            ID = Utils.Latin1Encoding.GetString(r.ReadBytes(4));
            StreamVersion = r.ReadByte();
            TypeFlag = r.ReadByte();
            AbsolutePosition = r.ReadUInt64();
            Serial = r.ReadInt32();
            PageNumber = r.ReadInt32();
            Checksum = r.ReadUInt32();
            Segments = r.ReadByte();
            LacingValues = r.ReadBytes(Segments);
        }

        public void ReadFromStream(BinaryReader r)
        {
            ID = Utils.Latin1Encoding.GetString(r.ReadBytes(4));
            StreamVersion = r.ReadByte();
            TypeFlag = r.ReadByte();
            AbsolutePosition = r.ReadUInt64();
            Serial = r.ReadInt32();
            PageNumber = r.ReadInt32();
            Checksum = r.ReadUInt32();
            Segments = r.ReadByte();
            LacingValues = r.ReadBytes(Segments);
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Utils.Latin1Encoding.GetBytes(ID));
            w.Write(StreamVersion);
            w.Write(TypeFlag);
            w.Write(AbsolutePosition);
            w.Write(Serial);
            w.Write(PageNumber);
            w.Write(Checksum);
            w.Write(Segments);
            w.Write(LacingValues);
        }

        public int GetPageLength()
        {
            int length = 0;
            for (int i = 0; i < Segments; i++)
            {
                length += LacingValues[i];
            }
            return length;
        }

        public int GetHeaderSize()
        {
            return 27 + LacingValues.Length;
        }

        public bool IsValid()
        {
            return ((ID != null) && ID.Equals(OGG_PAGE_ID));
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string downloads = @"C:\Users\StefHeyenrath\Downloads\";

            

            //var infoOgg = VorbisInfo.InitVariableBitRate(2, 48000, 0.5f);
            //var oggStream = new OggStream(0);

            //var infoPacket = HeaderPacketBuilder.BuildInfoPacket(infoOgg);
            ////var commentsPacket = HeaderPacketBuilder.BuildCommentsPacket(comments);

            //oggStream.PacketIn(infoPacket);
            //// oggStream.PacketIn(commentsPacket);

            //// Flush to force audio data onto its own page per the spec
            //OggPage page;
            //byte[] h;
            ////while (oggStream.PageOut(out page, true))
            //oggStream.PageOut(out page, true);
            //{
            //    h = page.Header;
                
            //    ms1.Write(h, 0, h.Length - 4);

            //    //ms1.Write(page.Header, 0, page.Header.Length);
            //    //ms1.Write(page.Body, 0, page.Body.Length);
            //}

            //ms1.Write(h, 0, h.Length - 4);
            // ms1.Write(new byte[4], 0, 4);

            //if (1 == 6)
            //{
            //    var br = new BinaryWriter(ms1);
            //    //br.Write(System.Text.Encoding.ASCII.GetBytes("OggS"));
            //    //br.Write((long)48000); // original input sample rate in Hz
            //    //br.Write((byte)2); // channel count

            //    br.Write(System.Text.Encoding.ASCII.GetBytes("OpusHead"));
            //    br.Write((byte)1); // version
            //    br.Write((byte)2); // channel count
            //    br.Write(new byte[2]); // pre-skip
            //    br.Write((long)48000); // original input sample rate in Hz 
            //    br.Write(new byte[2]); // output gain Q7.8 in dB
            //    br.Write((byte)0); // mapping
            //    br.Write((long)0); // ?
            //    br.Write(System.Text.Encoding.ASCII.GetBytes("OpusTags"));
            //    br.Write(0L); // ?
            //    br.Write(0L); // ?
            //   // br.Write(h, 0, page.Header.Length); // OggS again ?
            //    br.Flush();
            //}

            //     ms1 = new MemoryStream();
            var org = File.OpenRead(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_track1_[eng]_DELAY 0ms.opus");

            var oggHeader1 = new OggHeader();
            var source = new BinaryReader(org);
            oggHeader1.ReadFromStream(source);

           // var memH = new MemoryStream();
            

            



            //var ID = Utils.Latin1Encoding.GetString(source.ReadBytes(8));
            ////var isValidHeader = OPUS_HEADER_ID.Equals(info.OpusParameters.ID);

            //var Version = source.ReadByte();
            //var OutputChannelCount = source.ReadByte();
            //var PreSkip = source.ReadUInt16();
            ////info.OpusParameters.InputSampleRate = source.ReadUInt32();
            //var InputSampleRate = 48000; // Actual sample rate is hardware-dependent. Let's assume for now that the hardware ATL runs on supports 48KHz
            //source.Seek(4, SeekOrigin.Current);
            //var OutputGain = source.ReadInt16();

            //var ChannelMappingFamily = source.ReadByte();

            // ms1.Write(org, 0, 123);

            //var dataStream = new FileStream(@"C:\Users\azurestef\Downloads\test1.mkv", FileMode.Open, FileAccess.Read);

            var dataStream = new FileStream(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).webm", FileMode.Open, FileAccess.Read);
           
            var reader = new NEbml.Core.EbmlReader(dataStream);

            reader.ReadNext();

            var segments = new List<Segment>();

            if (reader.LocateElement(MatroskaDtd.Segment))
            {
                var segment = Segment.Read(reader);

                Console.WriteLine(JsonSerializer.Serialize(segment.Info, new JsonSerializerOptions { WriteIndented = true }));
                Console.WriteLine(JsonSerializer.Serialize(segment.Tracks, new JsonSerializerOptions { WriteIndented = true }));

                var ms1 = new MemoryStream();
                var oggHeader = new BinaryWriter(ms1);

                // OggS
                oggHeader1.WriteToStream(oggHeader);

                // opus
                ms1.Write(segment.Tracks.TrackEntry.CodecPrivate);

                // OggS again?
                oggHeader1.WriteToStream(oggHeader);

                foreach (var cluster in segment.Clusters)
                {
                    if (ms1.Position > 4210)
                    {
                        continue;
                    }

                    // ms1.Write(System.Text.Encoding.ASCII.GetBytes("X"));
                    foreach (var b in cluster.SimpleBlocks)
                    {
                        if (ms1.Position > 4210)
                        {
                            continue;
                        }

                        ms1.Write(b.Data);

                        
                    }
                }

                File.WriteAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus", ms1.ToArray());
                return;

                // Console.WriteLine(JsonSerializer.Serialize(segment, new JsonSerializerOptions { WriteIndented = true }));

                //reader.EnterContainer();

                //if (reader.LocateElement(MatroskaDtd.Segment.Info))
                //{
                //    var i2 = new Info();
                //    i2.Parse(reader);
                //    Console.WriteLine(JsonSerializer.Serialize(i2, new JsonSerializerOptions { WriteIndented = true }));
                //}



                uint oggPacket = 0;
                while (reader.ReadNext())
                {
                    if (reader.ElementId == MatroskaDtd.Segment.Cluster.Identifier)
                    {
                        reader.EnterContainer();

                        

                        

                        while (reader.ReadNext())
                        {
                            if (reader.ElementId == MatroskaDtd.Segment.Cluster.Timecode.Identifier)
                            {
                                var tc = reader.ReadUInt();
                                Console.WriteLine("tc: " + tc);

                                //int len = (int)reader.ElementSize;
                                //var buffer = ArrayPool<byte>.Shared.Rent(len);
                                //var read = reader.ReadBinary(buffer, 0, len);

                                //ms1.Write(buffer, 0, read);

                                

                                //var memH = new MemoryStream();
                                //var oggHeader = new BinaryWriter(memH);

                                //oggHeader1.WriteToStream(oggHeader);

                                //// 4F	67	67	53	00	2	00	00	00	00	00	00	00	00	8E	2B	E00	BA	00	00	00	00


                                //oggHeader.Write(System.Text.Encoding.ASCII.GetBytes("OggS"));
                                //oggHeader.Write((byte)0); // Header type – 8 bits
                                //oggHeader.Write((byte)0); // start
                                //oggHeader.Write((ulong)0); // gr
                                //oggHeader.Write(0x8E); // 8E 2B E0 BA 
                                //oggHeader.Write(0x2B); // 8E 2B E0 BA 
                                //oggHeader.Write(0xE0); // 8E 2B E0 BA 
                                //oggHeader.Write(0xBA); // 8E 2B E0 BA 
                                //oggHeader.Write(oggPacket); // packet
                                //oggHeader.Write(oggPacket); // crc
                                //oggHeader.Flush();

                                //memH.Position = 0;
                                //memH.CopyTo(ms1);

                                oggPacket++;
                            }
                            else if (reader.ElementId == MatroskaDtd.Segment.Cluster.SimpleBlock.Identifier)
                            {
                                int len = (int)reader.ElementSize;
                                var buffer = ArrayPool<byte>.Shared.Rent(len);
                                var read = reader.ReadBinary(buffer, 0, len);

                                using var mem = new MemoryStream(buffer, 0, read);
                                mem.ReadByte();
                                mem.ReadByte();

                                
                                //var readerBin = new NEbml.Core.EbmlReader(mem);
                                //readerBin.ReadInt();

                                // var tr = NEbml.Core.VInt.Read(mem, 8, ArrayPool<byte>.Shared.Rent(8));
                                //Console.WriteLine("Track Number (Track Entry): " + tr);
                                // Track Number (Track Entry). It is coded in EBML like form (1 octet if the value is < 0x80, 2 if < 0x4000, etc) (most significant bits set to increase the range).

                                //var ts = NEbml.Core.VInt.Read(mem, 8, ArrayPool<byte>.Shared.Rent(8));
                                //var ts = binr.ReadInt16(); // Timestamp (relative to Cluster timestamp, signed int16)
                                //

                                using var brr = new BinaryReader(mem);
                                var ts = brr.ReadInt16();
                                //Console.WriteLine(ts);

                                mem.CopyTo(ms1);

                                

                                //ms1.Write(mem, 4, read-4);

                                //reader.EnterContainer();

                                //while (reader.ReadNext())
                                //{
                                //    Console.WriteLine(reader.GetName());
                                //}

                                // Console.WriteLine(reader.GetName());
                                //int len = (int)reader.ElementSize;
                                ////if (len >= 8)
                                //{
                                //    var buffer = ArrayPool<byte>.Shared.Rent(len);
                                //    var read = reader.ReadBinary(buffer, 0, len);

                                //    ms1.Write(buffer, 0, read);
                                //}

                                int yy88 = 0;

                                //reader.LeaveContainer();
                            }
                            else if (reader.ElementId == MatroskaDtd.Segment.Cluster.BlockGroup.Identifier)
                            {
                                reader.EnterContainer();

                                while (reader.ReadNext())
                                {
                                    Console.WriteLine(reader.GetName());
                                }

                                reader.LeaveContainer();
                            }
                        }

                        reader.LeaveContainer();
                    }
                    else
                    {
                        Console.WriteLine("other:" + reader.GetName());
                    }
                }
            }

           // File.WriteAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus", ms1.ToArray());
            return;

            // var e = EbmlElement.ReadElements(file).ToList();

            //MatroskaParser p = new MatroskaParser();
            //p.Parse(file);

            var ms = new MemoryStream();
            /*
            var file = new FileStream(@"C:\Users\Heyenrath SWW\Downloads\Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).webm", FileMode.Open, FileAccess.Read);
            foreach (var top in EbmlElement.ReadElements(file))
            {
                Console.WriteLine(top.GetName());

                if (top.ID == MatroskaSpecificationEbml.Segment.ID)
                {


                    //var els = EbmlElement.ReadElements(ebmlData).ToList();

                    foreach (EbmlElement element in EbmlElement.ReadElements(top))
                    {
                        Console.WriteLine(element.GetName());

                        if (element.ID == MatroskaSpecificationEbml.SeekHead.ID)
                        {
                            foreach (EbmlElement seekHeadElement in EbmlElement.ReadElements(element))
                            {
                                Console.WriteLine("SeekHead: " + seekHeadElement.GetName());
                            }
                        }

                        if (element.ID == MatroskaSpecificationEbml.Info.ID)
                        {
                            var info = new Info();

                            await info.ParseAsync(element);

                            Console.WriteLine(JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true }));
                        }

                        if (element.ID == MatroskaSpecificationEbml.Tracks.ID)
                        {
                            var trackEntry = EbmlElement.ReadElement(element);
                            foreach (EbmlElement trackEntryElement in EbmlElement.ReadElements(trackEntry))
                            {
                                Console.WriteLine(trackEntryElement.GetName());

                                if (trackEntryElement.ID == MatroskaSpecificationEbml.TrackNumber.ID)
                                {
                                    Console.WriteLine("TrackNumber: " + trackEntryElement.ReadUnsignedInteger());
                                }

                                if (trackEntryElement.ID == MatroskaSpecificationEbml.CodecID.ID)
                                {
                                    Console.WriteLine("CodecID: " + trackEntryElement.ReadString());
                                }

                                if (trackEntryElement.ID == MatroskaSpecificationEbml.CodecName.ID)
                                {
                                    Console.WriteLine("CodecName: " + trackEntryElement.ReadUTF8());
                                }

                                if (trackEntryElement.ID == MatroskaSpecificationEbml.Audio.ID)
                                {
                                    //var audio = EbmlElement.ReadElement(trackEntryElement);
                                    foreach (EbmlElement audioElement in EbmlElement.ReadElements(trackEntryElement))
                                    {
                                        Console.WriteLine("audioElement: " + audioElement.GetName());
                                        if (audioElement.ID == MatroskaSpecificationEbml.SamplingFrequency.ID)
                                        {
                                            Console.WriteLine("SamplingFrequency : " + audioElement.ReadFloat());
                                        }

                                        if (audioElement.ID == MatroskaSpecificationEbml.BitDepth.ID)
                                        {
                                            Console.WriteLine("BitDepth : " + audioElement.ReadUnsignedInteger());
                                        }

                                        if (audioElement.ID == MatroskaSpecificationEbml.Channels.ID)
                                        {
                                            Console.WriteLine("Channels : " + audioElement.ReadUnsignedInteger());
                                        }
                                    }
                                }
                            }
                        }

                        if (element.ID == MatroskaSpecificationEbml.Cluster.ID)
                        {
                            foreach (EbmlElement clusterElement in EbmlElement.ReadElements(element))
                            {
                                // Console.WriteLine(clusterElement);

                                if (clusterElement.ID == MatroskaSpecificationEbml.Timecode.ID)
                                {
                                    Console.WriteLine("Timecode: " + clusterElement.ReadUnsignedInteger());
                                }

                                if (clusterElement.ID == MatroskaSpecificationEbml.SimpleBlock.ID)
                                {
                                    int len = (int)clusterElement.Length;
                                    var buffer = ArrayPool<byte>.Shared.Rent(len);

                                    int read = clusterElement.Read(buffer);

                                    await ms.WriteAsync(buffer, 0, read);
                                }
                            }
                        }
                    }
                }
            }

            //EbmlElement ebmlHeader = EbmlElement.ReadElement(file);
            //Console.WriteLine(ebmlHeader.ToString());

            //foreach (EbmlElement element in EbmlElement.ReadElements(ebmlHeader))
            //{
            //    Console.WriteLine(element.GetName());
            //}

            //EbmlElement ebmlData = EbmlElement.ReadElement(file);



            int yend = 0;

            var data = ms.ToArray();

            //var theTrack1 = new Track(ms, "audio/ogg");


            //var theTrack = new Track(@"C:\Users\Heyenrath SWW\Downloads\Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_track1_[eng]_DELAY 0ms.opus");
            //theTrack.AdditionalFields = new Dictionary<string, string>();

            //theTrack.Save();


            var msHeader = new MemoryStream();

            //using (FileStream fileOut = new FileStream(@"C:\Users\Heyenrath SWW\Downloads\Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus", FileMode.Create))
            //{
            //    OpusEncoder encoder = OpusEncoder.Create(48000, 2, OpusApplication.OPUS_APPLICATION_AUDIO);


            //    //encoder.Bitrate = 96000;

            //    OpusTags tags = new OpusTags();
            //    tags.Fields[OpusTagName.Title] = "Prisencolinensinainciusol";
            //    tags.Fields[OpusTagName.Artist] = "Adriano Celetano";
            //    OpusOggWriteStream oggOut = new OpusOggWriteStream(encoder, msHeader, tags);

            //    // byte[] allInput = File.ReadAllBytes(rawFile);
            //    short[] samples = data.Select(d => (short) d).ToArray();  //BytesToShorts(data);

            //    //oggOut.WriteSamples(samples, 0, samples.Length);
            //    oggOut.Finish();
            //}

            //var ms3 = new MemoryStream();
            //ms3.Write(msHeader.ToArray());
            //ms3.Write(ms.ToArray());

            ISoundOut soundOut;
            if (WasapiOut.IsSupportedOnCurrentPlatform)
            {
                soundOut = new WasapiOut();
            }
            else
            {
                soundOut = new DirectSoundOut();
            }

            //var stream = File.OpenRead(@"c:\temp\Abandoned _ Chill Mix.opus");
            // var track = new Track(stream, ".opus");

            var waveSource = new OpusSource(ms, 48000, 2);

            Console.WriteLine("len = {0} {1}", waveSource.Length, waveSource.GetLength());

            soundOut.Initialize(waveSource);
            soundOut.Play();
            */
            // File.WriteAllBytes(@"C:\Users\Heyenrath SWW\Downloads\Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus", ms.ToArray());
        }

        /// <summary>
        /// Converts interleaved byte samples (such as what you get from a capture device)
        /// into linear short samples (that are much easier to work with)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static short[] BytesToShorts(byte[] input)
        {
            return BytesToShorts(input, 0, input.Length);
        }

        /// <summary>
        /// Converts interleaved byte samples (such as what you get from a capture device)
        /// into linear short samples (that are much easier to work with)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static short[] BytesToShorts(byte[] input, int offset, int length)
        {
            short[] processedValues = new short[length / 2];
            for (int c = 0; c < processedValues.Length; c++)
            {
                processedValues[c] = (short)(((int)input[(c * 2) + offset]) << 0);
                processedValues[c] += (short)(((int)input[(c * 2) + 1 + offset]) << 8);
            }

            return processedValues;
        }
    }
}
