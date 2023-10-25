﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MaskedTextBoxAccessibilityObjectTests
{
    [WinFormsFact]
    public void MaskedTextBoxAccessibilityObject_Ctor_Default()
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.CreateControl();

        Assert.NotNull(maskedTextBox.AccessibilityObject);
        Assert.True(maskedTextBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void MaskedTextBoxAccessibilityObject_ControlType_IsEdit_IfAccessibleRoleIsDefault()
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.CreateControl();
        // AccessibleRole is not set = Default

        object actual = maskedTextBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_EditControlTypeId, actual);
        Assert.True(maskedTextBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void MaskedTextBoxAccessibilityObject_Role_IsText_ByDefault()
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.CreateControl();
        // AccessibleRole is not set = Default

        AccessibleRole actual = maskedTextBox.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.Text, actual);
        Assert.True(maskedTextBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> MaskedTextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
    {
        Array roles = Enum.GetValues(typeof(AccessibleRole));

        foreach (AccessibleRole role in roles)
        {
            if (role == AccessibleRole.Default)
            {
                continue; // The test checks custom roles
            }

            yield return new object[] { role };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MaskedTextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void MaskedTextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.AccessibleRole = role;

        object actual = maskedTextBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(maskedTextBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("Test", "Test")]
    public void MaskedTextBoxAccessibleObject_Name_IsExpected_WithoutMask(string accessibleName, string expectedAccessibleName)
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.Text = "000000";
        maskedTextBox.AccessibleName = accessibleName;

        var actual = (string)maskedTextBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId);

        Assert.Equal(expectedAccessibleName, actual);
        Assert.False(maskedTextBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("Test", "Test")]
    public void MaskedTextBoxAccessibleObject_Name_IsExpected_WithMask(string accessibleName, string expectedAccessibleName)
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.Text = "000000";
        maskedTextBox.Mask = "00/00/0000";
        maskedTextBox.AccessibleName = accessibleName;

        var actual = (string)maskedTextBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId);

        Assert.Equal(expectedAccessibleName, actual);
        Assert.False(maskedTextBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void MaskedTextBoxAccessibleObject_GetPropertyValue_Value_IsExpected_WithMask(bool useMask)
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.Text = "000000";
        maskedTextBox.Mask = useMask ? "00/00/0000" : null;

        var actual = (string)maskedTextBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId);

        Assert.Equal(maskedTextBox.WindowText, actual);
        Assert.Equal(useMask, maskedTextBox.Mask?.Length == actual.Length);
        Assert.False(maskedTextBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void MaskedTextBoxAccessibleObject_GetPropertyValue_Value_AccessDenied_WithUseSystemPasswordChar()
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.UseSystemPasswordChar = true;
        maskedTextBox.Text = "some text";

        object actual = maskedTextBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId);

        Assert.Equal(SR.AccessDenied, actual);
        Assert.False(maskedTextBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void MaskedTextBoxAccessibleObject_IsPassword_IsExpected_WithUseSystemPasswordChar(bool useSystemPasswordChar)
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.UseSystemPasswordChar = useSystemPasswordChar;

        object actual = maskedTextBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId);

        Assert.Equal(useSystemPasswordChar, actual);
        Assert.False(maskedTextBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData('\0')]
    [InlineData('*')]
    public void MaskedTextBoxAccessibleObject_IsPassword_IsExpected_WithPasswordChar(char passwordChar)
    {
        using MaskedTextBox maskedTextBox = new MaskedTextBox();
        maskedTextBox.PasswordChar = passwordChar;

        object actual = maskedTextBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId);
        bool expected = passwordChar != '\0';

        Assert.Equal(expected, actual);
        Assert.False(maskedTextBox.IsHandleCreated);
    }
}