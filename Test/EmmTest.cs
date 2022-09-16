using MmdMapMaid.Core.Models.Emm;
using NuGet.Frameworks;

namespace Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void EMM�ގ��������ёւ��e�X�g()
    {
        var mapper = new EmmOrderMapper(TestData.GetPath("MaterialOrderTestSource.emm"));

        var generatedPath = mapper.Run(TestData.GetPath("RGBBox_order1.pmx"), TestData.GetPath("RGBBox_order2.pmx"), new int[] {0}, new(false, false, TestData.GeneratedDirectory));
        if(generatedPath is null)
        {
            Assert.Fail("�����t�@�C���̃p�X���Ԃ��Ă��Ă��܂���B");
            return;
        }

        var expected = File.ReadAllText(TestData.GetPath("MaterialOrderTestExpected.emm"));
        var generated = File.ReadAllText(generatedPath);
        Assert.That(generated, Is.EqualTo(expected));
    }
}