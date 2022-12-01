using CSCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core.BiQuad
{
    public class BiQuadFilterSource : SampleAggregatorBase
    {

        //  VARIABLES

        private CSCore.DSP.BiQuad _biquad;
        private readonly object _lockObject = new object();


        //  GETTERS & SETTERS

        public CSCore.DSP.BiQuad Filter
        {
            get { return _biquad; }
            set
            {
                lock (_lockObject)
                {
                    _biquad = value;
                }
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BiQuadFilterSource class constructor. </summary>
        /// <param name="source"> Sample source. </param>
        public BiQuadFilterSource(ISampleSource source) : base(source)
        {

        }

        #endregion CLASS METHODS

        #region BUFFER WORKFLOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Read sample from buffer. </summary>
        /// <param name="buffer"> Buffer array. </param>
        /// <param name="offset"> Offset in bufer. </param>
        /// <param name="count"> Buffer length to read. </param>
        /// <returns> Count of samples readed. </returns>
        public override int Read(float[] buffer, int offset, int count)
        {
            int read = base.Read(buffer, offset, count);
            lock (_lockObject)
            {
                if (Filter != null)
                {
                    for (int i = 0; i < read; i++)
                    {
                        buffer[i + offset] = Filter.Process(buffer[i + offset]);
                    }
                }
            }

            return read;
        }

        #endregion BUFFER WORKFLOW METHODS

    }
}
