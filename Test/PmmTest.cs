using MikuMikuMethods.Mme;
using MmdMapMaid.Core.Models.Pmm;

namespace Test;
internal class PmmTest
{
    [Test]
    public void EMM同時編集テスト()
    {
        var replacer = new PmmPathReplacer(TestData.GetPath("PmmEmmPathTest.pmm"));

        var newPath1 = "UserFile\\Model\\初音ミク.pmx";
        replacer.ReplaceModelPath(0, newPath1, true);
        var newPath2 = "C:\\MMD\\_Models\\_508\\初音ミク\\miku_v3.pmx";
        replacer.ReplaceModelPath(1, newPath2, true);

        var savePath = replacer.Save(new(false, false, TestData.GeneratedDirectory, DateTime.Now));

        Assert.That(savePath, Is.Not.Null);
        var gEmmName = Path.GetFileNameWithoutExtension(savePath) + ".emm";

        var gEmmPath = TestData.GetGeneratedPath(gEmmName);
        Assert.That(File.Exists(gEmmPath), Is.True);

        var gEmm = new EmmData(gEmmPath);
        Assert.Multiple(() =>
        {
            Assert.That(gEmm.Objects[0].Path, Is.EqualTo(newPath1));
            Assert.That(gEmm.Objects[1].Path, Is.EqualTo(newPath2));
        });
    }
}
