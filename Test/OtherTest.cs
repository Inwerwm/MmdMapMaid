using MmdMapMaid.Core.Helpers;

namespace Test;
internal class OtherTest
{
    [Test]
    public void TakeCyclicTest()
    {
        var ar = new int[] { 1, 2 };

        var result = ar.TakeCyclic().Take(4);

        CollectionAssert.AreEqual(new[] { 1, 2, 1, 2 }, result);
    }
}
