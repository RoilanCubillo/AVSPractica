using System;
using System.Globalization;
using System.Web.Mvc;

namespace UltraERP.Services
{
    public class FlexibleDecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bool isNullable = Nullable.GetUnderlyingType(bindingContext.ModelType) != null;

            if (valueResult == null)
                return isNullable ? null : (object)0m;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);

            string attemptedValue = (valueResult.AttemptedValue ?? "").Trim();
            if (String.IsNullOrWhiteSpace(attemptedValue))
                return isNullable ? null : (object)0m;

            decimal parsedValue;
            if (TryParseDecimal(attemptedValue, out parsedValue))
                return parsedValue;

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Ingrese un numero valido.");
            return isNullable ? null : (object)0m;
        }

        private static bool TryParseDecimal(string value, out decimal result)
        {
            value = (value ?? "").Trim();
            if (String.IsNullOrWhiteSpace(value))
            {
                result = 0m;
                return true;
            }

            string normalized = value.Replace(" ", "");
            bool hasComma = normalized.IndexOf(",") >= 0;
            bool hasDot = normalized.IndexOf(".") >= 0;

            if (hasComma && hasDot && normalized.LastIndexOf(",") > normalized.LastIndexOf("."))
                normalized = normalized.Replace(".", "").Replace(",", ".");
            else if (hasComma && hasDot)
                normalized = normalized.Replace(",", "");
            else if (hasComma)
                normalized = normalized.Replace(",", ".");

            if (Decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                return true;

            if (Decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                return true;

            return Decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out result);
        }
    }
}
