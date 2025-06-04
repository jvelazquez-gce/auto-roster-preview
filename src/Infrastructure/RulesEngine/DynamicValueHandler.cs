using Domain.Entities;
using System;

namespace Infrastructure.RulesEngine
{
    public static class DynamicValueHandler
    {
        public static bool IsRuleValid(PreLoadStudentSection preLoadStudentSection, Rule rule)
        {
            var fieldExpectedValueTypeProperty = preLoadStudentSection.GetType().GetProperty(rule.FieldExpectedValueType);
            var fieldExpectedValueType = fieldExpectedValueTypeProperty.GetValue(preLoadStudentSection);
            var fieldNameroperty = preLoadStudentSection.GetType().GetProperty(rule.FieldName);

            switch (fieldExpectedValueType)
            {
                case "int":
                    var intActualFieldNameValue = (int)fieldNameroperty.GetValue(preLoadStudentSection);
                    var intFieldExpectedValue = int.Parse(rule.FieldExpectedValue);
                    return intActualFieldNameValue == intFieldExpectedValue;
                case "string":
                    var strActualFieldNameValue = (string)fieldNameroperty.GetValue(preLoadStudentSection);
                    return strActualFieldNameValue.Equals(rule.FieldExpectedValue, StringComparison.CurrentCultureIgnoreCase);
                case "bool":
                    var boolActualFieldNameValue = (bool)fieldNameroperty.GetValue(preLoadStudentSection);
                    var boolFieldExpectedValue = bool.Parse(rule.FieldExpectedValue);
                    return boolActualFieldNameValue == boolFieldExpectedValue;
                case "double":
                    var doubleActualFieldNameValue = (double)fieldNameroperty.GetValue(preLoadStudentSection);
                    var doubleFieldExpectedValue = double.Parse(rule.FieldExpectedValue);
                    return doubleActualFieldNameValue == doubleFieldExpectedValue;
                case "decimal":
                    var decimalActualFieldNameValue = (decimal)fieldNameroperty.GetValue(preLoadStudentSection);
                    var decimalFieldExpectedValue = decimal.Parse(rule.FieldExpectedValue);
                    return decimalActualFieldNameValue == decimalFieldExpectedValue;
                default:
                    return false;
            }
        }
    }
}
