using System;
using System.Collections;
using System.Collections.Generic;

public class APIResponse
{
    public bool success;
    public int code;
    public IList<DongData> data;
}

public class DongData
{
    public IList<RoomType> roomtypes;

    public class Meta
    {
        public int bd_id;
        public string 동;
        public int 지면높이;
    }
    public Meta meta;
}

public class RoomType
{
    public string[] coordinatesBase64s;
    public class Meta
    {
        public int room_type;
    }
    public Meta meta;
}