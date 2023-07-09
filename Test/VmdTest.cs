using System.Numerics;
using MikuMikuMethods.Vmd;
using MmdMapMaid.Core.Models.Vmd;

namespace Test;

internal class VmdTest
{
    [Test]
    public void ScaleOffset_ShouldScalePositionAndRotation_ForVmdMotionFrame()
    {
        // Arrange
        var frames = new List<IVmdFrame>
    {
        new VmdMotionFrame("test", 1)
        {
            Position = new Vector3(1, 1, 1),
            Rotation = Quaternion.CreateFromYawPitchRoll(0, 0, 0)
        },
        new VmdMotionFrame("test", 2)
        {
            Position = new Vector3(2, 2, 2),
            Rotation = Quaternion.CreateFromYawPitchRoll(90, 0, 0)
        }
    };

        var scale = 0.5f;

        // Act
        var result = VmdRangeEditor.ScaleOffset(frames, scale).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result[0] is VmdMotionFrame);
            Assert.That(result[1] is VmdMotionFrame);
        });
        var secondFrame = result[1] as VmdMotionFrame;
        Assert.That(secondFrame, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(secondFrame.Position, Is.EqualTo(new Vector3(1.5f, 1.5f, 1.5f)));
            QuaternionAssert.AreEqual(Quaternion.CreateFromYawPitchRoll(45, 0, 0), secondFrame.Rotation);
        });
    }


    [Test]
    public void ScaleOffset_ShouldScalePositionAndRotation_ForVmdCameraFrame()
    {
        // Arrange
        var frames = new List<IVmdFrame>
        {
            new VmdCameraFrame(1)
            {
                Position = new Vector3(1, 1, 1),
                Rotation = new Vector3(1, 1, 1)
            },
            new VmdCameraFrame(2)
            {
                Position = new Vector3(2, 2, 2),
                Rotation = new Vector3(2, 2, 2)
            }
        };

        var scale = 0.5f;

        // Act
        var result = VmdRangeEditor.ScaleOffset(frames, scale).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result[0] is VmdCameraFrame);
            Assert.That(result[1] is VmdCameraFrame);
        });
        var secondFrame = result[1] as VmdCameraFrame;
        Assert.That(secondFrame, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(secondFrame.Position, Is.EqualTo(new Vector3(1.5f, 1.5f, 1.5f)));
            Assert.That(secondFrame.Rotation, Is.EqualTo(new Vector3(1.5f, 1.5f, 1.5f)));
        });
    }

    [Test]
    public void GenerateAlignedFramesTest()
    {
        // Arrange
        var guide = new List<IVmdFrame>
        {
            new VmdMotionFrame("guide", 0),
            new VmdMotionFrame("guide", 5),
            new VmdMotionFrame("guide", 10),
            new VmdMotionFrame("guide", 15)
        };

        var source = new List<IVmdFrame>
        {
            new VmdMotionFrame("a", 1),
            new VmdMotionFrame("a", 4),
            new VmdMotionFrame("b", 0),
            new VmdMotionFrame("b", 6),
            new VmdMotionFrame("b", 9),
        };

        // Act
        var result = VmdRangeEditor.GenerateAlignedFrames(source, guide).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(8));

            Assert.That(result[0].Name, Is.EqualTo("a"));
            Assert.That(result[0].Frame, Is.EqualTo(1));

            Assert.That(result[1].Name, Is.EqualTo("a"));
            Assert.That(result[1].Frame, Is.EqualTo(4));

            Assert.That(result[2].Name, Is.EqualTo("a"));
            Assert.That(result[2].Frame, Is.EqualTo(11));

            Assert.That(result[3].Name, Is.EqualTo("a"));
            Assert.That(result[3].Frame, Is.EqualTo(14));

            Assert.That(result[4].Name, Is.EqualTo("b"));
            Assert.That(result[4].Frame, Is.EqualTo(0));

            Assert.That(result[5].Name, Is.EqualTo("b"));
            Assert.That(result[5].Frame, Is.EqualTo(6));

            Assert.That(result[6].Name, Is.EqualTo("b"));
            Assert.That(result[6].Frame, Is.EqualTo(9));

            Assert.That(result[7].Name, Is.EqualTo("b"));
            Assert.That(result[7].Frame, Is.EqualTo(15));
        });
    }

}
