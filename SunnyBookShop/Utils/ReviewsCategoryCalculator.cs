namespace SunnyBookShop.Utils;

public class ReviewsCategoryCalculator
{
    public static string GetCategory(int? reviewsCount, int? positivePercents)
    {
        string reviewCategory = string.Empty;
        if (positivePercents > 94)
            reviewCategory = "Overhwelmingly Positive";
        else if (positivePercents > 79 && positivePercents < 95)
            reviewCategory = "Very Positive";
        else if (positivePercents > 69 && positivePercents < 80)
            reviewCategory = "Mostly Positive";
        else if (positivePercents > 39 && positivePercents < 70)
            reviewCategory = "Mixed";
        else if (positivePercents > 19 && positivePercents < 40)
            reviewCategory = "Mostly Negative";
        else if (positivePercents >= 0 && positivePercents < 20)
            reviewCategory = "Very Negative";
        if (reviewsCount == 0)
            reviewCategory = "No reviews";
        return reviewCategory;
    }
}