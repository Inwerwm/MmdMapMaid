using System.Numerics;

namespace Test;
public static class QuaternionAssert
{
    public static void AreEqual(Quaternion expected, Quaternion actual, float within = 0.0001f)
    {
        if (Quaternion.Dot(expected, actual) < 0)
        {
            actual = Quaternion.Negate(actual);
        }

        Assert.Multiple(() =>
        {
            Assert.That(actual.X, Is.EqualTo(expected.X).Within(within));
            Assert.That(actual.Y, Is.EqualTo(expected.Y).Within(within));
            Assert.That(actual.Z, Is.EqualTo(expected.Z).Within(within));
            Assert.That(actual.W, Is.EqualTo(expected.W).Within(within));
        });
    }
}
