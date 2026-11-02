using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace LEGOMaterials.Editor.Tests
{
    class ColourPickerFilterTests
    {
        [Test]
        public void EveryColourHasALoadableMaterial()
        {
            foreach (var id in MouldingColour.GetAllColourIDs())
            {
                Assert.That(MaterialUtility.LoadMaterial(id, MouldingColour.IsLegacy(id)),
                    Is.Not.Null, "Missing material for " + id);
            }
        }

        [Test]
        public void BrightYellowCanBeExcludedAndRestoredWithoutDuplicates()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            var picker = typeof(MouldingColourPicker);
            var coloursField = picker.GetField("universalColours", flags);
            var showingField = picker.GetField("showingBrightYellow", flags);
            var originalColours = coloursField.GetValue(null);
            var originalShowing = showingField.GetValue(null);
            var colours = new List<MouldingColour.Id> { MouldingColour.Id.BrightYellow };
            try
            {
                coloursField.SetValue(null, colours);
                showingField.SetValue(null, true);
                var exclude = picker.GetMethod("ExcludeBrightYellow", flags);
                var include = picker.GetMethod("IncludeBrightYellow", flags);
                exclude.Invoke(null, null);
                exclude.Invoke(null, null);
                Assert.That(colours, Is.Empty);
                Assert.That(showingField.GetValue(null), Is.False);
                include.Invoke(null, null);
                include.Invoke(null, null);
                Assert.That(colours, Is.EqualTo(new[] { MouldingColour.Id.BrightYellow }));
                Assert.That(showingField.GetValue(null), Is.True);
            }
            finally
            {
                coloursField.SetValue(null, originalColours);
                showingField.SetValue(null, originalShowing);
            }
        }
    }
}
