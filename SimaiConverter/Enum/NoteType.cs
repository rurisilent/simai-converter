using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaiConverter.Enum
{
    public enum NoteType
    {
        /// <summary>
        /// Tap
        /// </summary>
        TAP,

        /// <summary>
        /// Hold
        /// </summary>
        HLD,

        /// <summary>
        /// Star (Slide Start)
        /// </summary>
        STR,

        /// <summary>
        /// Touch
        /// </summary>
        TTP,

        /// <summary>
        /// Touch Hold
        /// </summary>
        THO,

        /// <summary>
        /// Straight Slide (-)
        /// </summary>
        SI_,

        /// <summary>
        /// Counter-clockwise Slide (NOT EQUAL to <)
        /// </summary>
        SCL,

        /// <summary>
        /// Clockwise Slide (NOT EQUAL to >)
        /// </summary>
        SCR,

        /// <summary>
        /// Center Crossing Slide (v)
        /// </summary>
        SV_,

        /// <summary>
        /// Wind Left Slide (p)
        /// </summary>
        SUL,

        /// <summary>
        /// Wind Right Slide (q)
        /// </summary>
        SUR,

        /// <summary>
        /// Wi-Fi Slide (w)
        /// </summary>
        SF_,

        /// <summary>
        /// Inflecting Left Slide (NOT EQUAL to V)
        /// </summary>
        SLL,

        /// <summary>
        /// Inflecting Right Slide (NOT EQUAL to V)
        /// </summary>
        SLR,

        /// <summary>
        /// Self-winding Left Slide (pp)
        /// </summary>
        SXL,

        /// <summary>
        /// Self-winding Right Slide (qq)
        /// </summary>
        SXR,

        /// <summary>
        /// Z Slide (z)
        /// </summary>
        SSL,

        /// <summary>
        /// S Slide (s)
        /// </summary>
        SSR
    }
}
