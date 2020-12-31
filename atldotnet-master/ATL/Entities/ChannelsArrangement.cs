﻿namespace ATL
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible members
    public class ChannelsArrangements
    {
        public static readonly ChannelsArrangement UNKNOWN = new ChannelsArrangement(0, "Unknown");

        // ISO/IEC 23001‐8 "Channel Configurations"
        public static readonly ChannelsArrangement ISO_1_0_0 = new ChannelsArrangement(1, "Mono (1/0.0)");
        public static readonly ChannelsArrangement MONO = ISO_1_0_0;
        public static readonly ChannelsArrangement ISO_2_0_0 = new ChannelsArrangement(2, "Stereo (2/0.0)");
        public static readonly ChannelsArrangement STEREO = ISO_2_0_0;
        public static readonly ChannelsArrangement ISO_3_0_0 = new ChannelsArrangement(3, "Center - Left - Right (3/0.0)");
        public static readonly ChannelsArrangement ISO_3_1_0 = new ChannelsArrangement(3, "Left - Right - Center - Center rear (3/1.0)");
        public static readonly ChannelsArrangement ISO_3_2_0 = new ChannelsArrangement(5, "Center - Left - Right - Left surround - Right surround (3/2.0)");
        public static readonly ChannelsArrangement ISO_3_2_1 = new ChannelsArrangement(6, "Center - Left - Right - Left surround - Right surround - Center front LFE (3/2.1) (aka 5.1)");
        public static readonly ChannelsArrangement ISO_5_2_1 = new ChannelsArrangement(6, "Center - Left center - Right center - Left - Right - Left surround - Right surround - Center front LFE (5/2.1)");
        public static readonly ChannelsArrangement ISO_1_1 = new ChannelsArrangement(2, "Dual Mono (1+1)");
        public static readonly ChannelsArrangement DUAL_MONO = ISO_1_1;
        public static readonly ChannelsArrangement ISO_2_1_0 = new ChannelsArrangement(3, "Left front - Right front - Center rear (2/1.0)");
        public static readonly ChannelsArrangement ISO_2_2_0 = new ChannelsArrangement(4, "Left front - Right front - Left rear - Right rear (2/2.0) (aka Quad)");
        public static readonly ChannelsArrangement QUAD = ISO_2_2_0;
        public static readonly ChannelsArrangement ISO_3_3_1 = new ChannelsArrangement(7, "Center front - Left front - Right front - Left surround - Right surround - Center rear - Center front LFE (3/3.1) (aka 6.1)");
        public static readonly ChannelsArrangement ISO_3_4_1 = new ChannelsArrangement(8, "Center front - Left front - Right front - Left surround - Right surround - Left rear - Right rear - Center front LFE (3/4.1)");
        public static readonly ChannelsArrangement ISO_11_11_2 = new ChannelsArrangement(24, "Center front - Left front - Right front - Left outside front - Right outside front - Left side - Right side - Left back - Right back - Center back - Left front LFE - Right front LFE - Top center front - Top left front - Top right front - Top left side - Top right side - Center Ceiling - Top left back - Top right back - Top center back - Bottom center front - Bottom left front - Bottom right front (11/11.2)");

        // Other common configurations
        public static readonly ChannelsArrangement JOINT_STEREO = new ChannelsArrangement(2, "Joint Stereo");
        public static readonly ChannelsArrangement JOINT_STEREO_INTENSITY = new ChannelsArrangement(2, "Joint Stereo - intensity");
        public static readonly ChannelsArrangement JOINT_STEREO_LEFT_SIDE = new ChannelsArrangement(2, "Joint Stereo - left/side");
        public static readonly ChannelsArrangement JOINT_STEREO_RIGHT_SIDE = new ChannelsArrangement(2, "Joint Stereo - right/side");
        public static readonly ChannelsArrangement JOINT_STEREO_MID_SIDE = new ChannelsArrangement(2, "Joint Stereo - mid/side");
        public static readonly ChannelsArrangement LRCS = new ChannelsArrangement(4, "Left - Right - Center - Surround");
        public static readonly ChannelsArrangement LRCLFECrLssRss = new ChannelsArrangement(7, "Left front - Right front - Center front - LFE - Center rear - Left side - Right side"); // 6.1 ?
        public static readonly ChannelsArrangement LRCLFELrRrLssRss = new ChannelsArrangement(8, "Left front - Right front - Center front - LFE - Left rear - Right rear - Left side - Right side"); // 7.1 ?

        // AIFF specifics
        public static readonly ChannelsArrangement LRLcRcCS = new ChannelsArrangement(6, "Left - Right - Left center - Right center - Center - Surround");

        // DSF specifics
        public static readonly ChannelsArrangement LRCLFE = new ChannelsArrangement(4, "Left - Right - Center - LFE");

        // DTS specifics
        public static readonly ChannelsArrangement STEREO_SUM_DIFFERENCE = new ChannelsArrangement(2, "Stereo - Sum & Difference");
        public static readonly ChannelsArrangement STEREO_LEFT_RIGHT_TOTAL = new ChannelsArrangement(2, "Stereo - Left & Right Total");
        public static readonly ChannelsArrangement CLCRLRSLSR = new ChannelsArrangement(6, "Left center - Right center - Left - Right - Left surround - Right surround");
        public static readonly ChannelsArrangement CLRLRRRO = new ChannelsArrangement(6, "Center - Left - Right - Left rear - Right rear - Overhead");
        public static readonly ChannelsArrangement CFCRLFRFLRRR = new ChannelsArrangement(6, "Center front - Center rear - Left front - Right front - Left rear - Right rear");
        public static readonly ChannelsArrangement CLCCRLRSLSR = new ChannelsArrangement(7, "Left center - Center - Right center - Left - Right - Left surround - Right surround");
        public static readonly ChannelsArrangement CLCRLRSL1SL2SR1SR2 = new ChannelsArrangement(8, "Left center - Right center - Left - Right - Left surround 1 - Left surround 2 - Right surround 1 - Right surround 2");
        public static readonly ChannelsArrangement CLCCRLRSLSSR = new ChannelsArrangement(8, "Left center - Center - Right center - Left - Right - Left surround - Surround - Right surround");


        /// <summary>
        /// Returns the most commonly used ISO ChannelsArrangement corresponding to the given number of channels
        /// </summary>
        /// <param name="nbChannels">Number of channels</param>
        /// <returns>Most commonly used ISO ChannelsArrangement corresponding to the given number of channels</returns>
        public static ChannelsArrangement GuessFromChannelNumber(int nbChannels)
        {
            switch (nbChannels)
            {
                case 1: return MONO;
                case 2: return STEREO;
                case 3: return ISO_3_0_0;
                case 4: return QUAD;
                case 5: return ISO_3_2_0;
                case 6: return ISO_3_2_1;
                case 7: return ISO_3_3_1;
                case 8: return ISO_3_4_1;
                default: return new ChannelsArrangement(nbChannels);
            }
        }

        /// <summary>
        /// Describes a specific arrangement of audio channels
        /// </summary>
        public class ChannelsArrangement
        {
            public ChannelsArrangement(int nbChannels, string description)
            {
                this.NbChannels = nbChannels;
                this.Description = description;
            }

            public ChannelsArrangement(int nbChannels)
            {
                this.NbChannels = nbChannels;
                this.Description = nbChannels + " channels";
            }

            public override string ToString() // Keep it there; used to get a clearer trace when testing
            {
                return Description;
            }

            /// <summary>
            /// Description of the arrangement, in english
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// Number of channels involved
            /// </summary>
            public int NbChannels { get; set; }
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible members