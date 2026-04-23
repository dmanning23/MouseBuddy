using Microsoft.Xna.Framework;
using Moq;
using MouseBuddy;
using NUnit.Framework;
using Shouldly;

namespace MouseBuddyTests
{
    [TestFixture]
    public class MouseManagerTests
    {
        public MouseManagerTests()
        {
        }

        [Test]
        public void Drag_DontMove()
        {
            var mouse = new Mock<MouseManager>(null) { CallBase = true };

            mouse.Object.LeftButtonState.ForceState((int)MouseButtonState.Dragging);
            mouse.Setup(x => x.LMouseDown).Returns(true);
            mouse.Setup(x => x.MousePos).Returns(new Vector2(0, 0));

            mouse.Object.Update(true);

            mouse.Object.LeftButtonState.CurrentState.ShouldBe((int)MouseButtonState.Dragging);
        }

        [Test]
        public void Drag_Move()
        {
            var mouse = new Mock<MouseManager>(null) { CallBase = true };

            mouse.Object.LeftButtonState.ForceState((int)MouseButtonState.Dragging);
            mouse.Setup(x => x.LMouseDown).Returns(true);
            mouse.Setup(x => x.MousePos).Returns(new Vector2(30, 0));

            mouse.Object.Update(true);

            mouse.Object.LeftButtonState.CurrentState.ShouldBe((int)MouseButtonState.Dragging);
        }
    }
}
