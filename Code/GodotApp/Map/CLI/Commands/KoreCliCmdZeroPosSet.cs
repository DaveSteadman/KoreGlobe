using System.Collections.Generic;

// KoreCommandEntityAdd
using KoreCommon;



public class KoreCliCmdZeroPosSet : KoreCommand
{
    public KoreCliCmdZeroPosSet()
    {
        Signature.Add("zeropos");
        Signature.Add("set");
    }

    public override string HelpString => $"{SignatureString} <Latitude degrees> <Longitude degrees> <Altitude Alt MSL meters>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count != 3)
        {
            return "KoreCliCmdZeroPosSet.Execute -> insufficient parameters";
        }

        if (!double.TryParse(parameters[0], out double lat) ||
            !double.TryParse(parameters[1], out double lon) ||
            !double.TryParse(parameters[2], out double alt))
        {
            return "KoreCliCmdZeroPosSet.Execute -> invalid parameter types";
        }

        KoreLLAPoint inputZeroPos = new KoreLLAPoint() { LatDegs = lat, LonDegs = lon, AltMslM = alt };
        KoreZeroOffset.SetLLA(inputZeroPos);
        
        return $"KoreCliCmdZeroPosSet.Execute -> Zero position set to: {lat}, {lon}, {alt}";
    }
}
