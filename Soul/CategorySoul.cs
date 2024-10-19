using System;
using System.Collections;
using System.Collections.Generic;

public class CategorySoul
{
    private CategorySoulData categorySoulData;

    private void SetCategorySoul(string id)
    {
        categorySoulData = new CategorySoulData();

        Dictionary<string, object> table = CSVReader.Read("MainCategorySoul")[id];
        categorySoulData.SetCategoryId(id);
        categorySoulData.SetOrder(Convert.ToInt32(table["SoulMainOrder"]));
        categorySoulData.SetCategoryName(Convert.ToString(table["SoulMainNameText"]));
        categorySoulData.SetImage(Convert.ToString(table["SoulMainImagePath"]));
        categorySoulData.SetSoulCount(Convert.ToInt32(table["SoulMainCount"]));
    }
}
