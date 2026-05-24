// Stub for Ocean Optics OmniDriver SDK (NETOmniDriver-NET40.dll).
// Allows the project to build without the SDK installed. USB2000 spectrometer
// will not actually function — calls throw NotSupportedException at runtime.
using System;

namespace OmniDriver
{
    public class CCoWrapper
    {
        public CCoWrapper() { }

        public void setStrobeEnable(int spectrometerIndex, int enable)
            => throw new NotSupportedException("OmniDriver SDK not installed.");

        public void setIntegrationTime(int spectrometerIndex, int microseconds)
            => throw new NotSupportedException("OmniDriver SDK not installed.");

        public int closeAllSpectrometers()
            => throw new NotSupportedException("OmniDriver SDK not installed.");

        public int openAllSpectrometers()
            => throw new NotSupportedException("OmniDriver SDK not installed.");

        public void setCorrectForElectricalDark(int spectrometerIndex, int enable)
            => throw new NotSupportedException("OmniDriver SDK not installed.");

        public void setScansToAverage(int spectrometerIndex, int scans)
            => throw new NotSupportedException("OmniDriver SDK not installed.");

        public double[] getSpectrum(int spectrometerIndex)
            => throw new NotSupportedException("OmniDriver SDK not installed.");

        public double[] getWavelengths(int spectrometerIndex)
            => throw new NotSupportedException("OmniDriver SDK not installed.");
    }
}
