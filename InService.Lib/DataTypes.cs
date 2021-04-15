using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib
{
    public enum DataTypes : int
    {
        INTEGER = 1,
        REAL_NUMBER = 2,
        DATE = 3,
        TIME = 4,
        SINGLE_LINE_TEXT = 5,
        MULTI_LINE_TEXT = 6,
        BOOLEAN = 7,
        EMAIL = 8,
        OPTIONS = 9,
        NAME = 10,
        PHONE_NUMBER = 11,
        PLACE_NAME = 12,
        STREET_ADDRESS = 13,
        CITY = 14,
        COUNTRY = 15,
        COUNTRY_LIST = 16,
        COUNTRY_CODE = 17,
        ENTITY_NAME = 18,
        MULTI_SELECT_OPTIONS = 19,
        THUMBNAIL = 20,
        //BANNER = 21,
        URL = 22,
        VAT_NUMBER = 23,
    }
}
