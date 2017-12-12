using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using UnityEngine;

/// <summary>
///     Result of Skobbler Direction API.
/// </summary>
public class OnlineMapsSkobblerDirectionsResult : JsonInterface<OnlineMapsSkobblerDirectionsResult>
{
    public List<Advisor> Advisors;
    public float Duration;
    public int HasFerries;
    public int HasHighWays;
    public int HasTolls;
    public float Routelength;
    /// <summary>
    /// Converts a list of the steps of the route to list of point locations.
    /// </summary>
    /// <param name="steps">List of the steps of the route.</param>
    /// <returns>A list of locations of route.</returns>
    public List<Vector2> GetPoints()
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < Advisors.Count; i++)
        {
            OnlineMapsSkobblerDirectionsResult.Advisor advisor = Advisors[i];
            points.Add(advisor.Coordinates);
        }
        return points;
    }
    public new static OnlineMapsSkobblerDirectionsResult FromJson(JsonData jsonData)
    {
        try
        {
            OnlineMapsSkobblerDirectionsResult data = new OnlineMapsSkobblerDirectionsResult();
            data.Advisors = Advisor.ArrayFromJson(jsonData["advisor"], Advisor.FromJson);
            data.Duration = int.Parse(jsonData["duration"].ToString());
            data.HasFerries = int.Parse(jsonData["hasFerries"].ToString());
            data.HasHighWays = int.Parse(jsonData["hasHighWays"].ToString());
            data.HasTolls = int.Parse(jsonData["hasTolls"].ToString());
            data.Routelength = float.Parse(jsonData["routelength"].ToString());

            return data;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public enum AdviceType
    {
        None = -1,
        DestinationPoint, //when the next advice is for exact point location, not street;
        DestinationStreet, // destination is a street;
        LastRouteItem, // we are on the last route item, but it is not the destination;
        DestinationRoundabout, // destination is in a roundabout;
        UTurn, // need to make a u-turn;
        Ignore, // if no advice should be said;
        EnterHighway, // will enter on highway;
        ExitHighway, // need to exit from highway;
        HighwayLink, // getting from one hway to other;
        CrossingHighway, // intersection between highways;
        Roundabout, // roundabout instruction;
        TStreet, // T-street instruction;
        StraightAhead, // route is straight ahead, when are multiple streets in crossing;
        StreetCrossing, // an intersection between other streets than highways;
        CarryStraightOn, // when no advice can be generated because lack of route items, or the distance to the advice is very big;
        ContinueDistStr, // informal advice to continue on a street for a certain distance(only US);
        ContinueDist, // informal advice to continue for a certain distance(only US);
        CountinueStraight, // informal advice to continue straight on(only US);
        RouteGuidance, // informal advice for starting the route(only US);
        RouteGuidanceStr // informal advice for starting the route from a certain street(only US);
    }

    public enum TurnDirection
    {
        None = -1,
        StraightAhead,
        SlightRight,
        SlightLeft,
        Left,
        Right,
        HardRight,
        HardLeft,
        UTurn,
        TStreet
    }

    public class Advisor : JsonInterface<Advisor>
    {
        public AdviceType Type;
        public Vector2 Coordinates;
        public int Distance;
        public string Instruction;
        public TurnDirection Direction;

        public Advisor()
        {
            Type = AdviceType.None;
            Direction = TurnDirection.None;
        }

        public new static Advisor FromJson(JsonData jsonData)
        {
            try
            {
                Advisor data = new Advisor();
                data.Type = (AdviceType)int.Parse(jsonData["adviceType"].ToString());

                JsonData vector2String = jsonData["coordinates"];
                data.Coordinates.x = float.Parse(vector2String["x"].ToString());
                data.Coordinates.y = float.Parse(vector2String["y"].ToString());

                data.Distance = int.Parse(jsonData["distance"].ToString());
                data.Instruction = jsonData["instruction"].ToString();

                if (jsonData.Keys.Contains("turnDirection"))
                    data.Direction = (TurnDirection)int.Parse(jsonData["turnDirection"].ToString());

                return data;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
    }
}