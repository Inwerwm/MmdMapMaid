using MmdMapMaid.Core.Models.Emm;

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
        Assert.That(generatedPath, Is.Not.Null, "�����t�@�C���̃p�X���Ԃ��Ă��Ă��܂���B");

        var expected = File.ReadAllText(TestData.GetPath("MaterialOrderTestExpected.emm"));
        var generated = File.ReadAllText(generatedPath);
        Assert.That(generated, Is.EqualTo(expected));
    }
}