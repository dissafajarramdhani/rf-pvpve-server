namespace RF.Server.Core.Models;

public sealed class WorldPosition
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public WorldPosition(double x = 0, double y = 0, double z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }
}
