using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuMethods.Mme;
using MmdMapMaid.Core.Models.Pmm;

namespace Test;
internal class PmmTest
{
    [Test]
    public void EMM同時編集テスト()
    {
        var newPath = "UserFile\\Model\\初音ミク.pmx";

        var replacer = new PmmPathReplacer(TestData.GetPath("PmmEmmPathTest.pmm"));
        replacer.ReplaceModelPath(0, newPath, true);
        var savePath = replacer.Save(new(false, false, TestData.GeneratedDirectory, DateTime.Now));

        Assert.That(savePath, Is.Not.Null);
        var gEmmName = Path.GetFileNameWithoutExtension(savePath) + ".emm";

        var gEmmPath = TestData.GetGeneratedPath(gEmmName);
        Assert.That(File.Exists(gEmmPath), Is.True);

        var gEmm = new EmmData(gEmmPath);
        Assert.That(gEmm.Objects[0].Path, Is.EqualTo(newPath));
    }
}
