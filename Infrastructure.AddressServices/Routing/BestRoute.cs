﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.DomainServices.RoutingClasses;
using Core.DomainServices.Ínterfaces;
using Infrastructure.AddressServices.Interfaces;
using log4net;
using log4net.Repository.Hierarchy;
using Address = Core.DomainModel.Address;
using Core.DomainModel;
using Core.DomainServices;

namespace Infrastructure.AddressServices.Routing
{
    public class BestRoute : IRoute<RouteInformation>
    {

        // Number of seconds a new route must be shorter to be considered a better route.
        private const int TimeThreshold = 300;
        // Distance in meters a new route must be shorter to be considered a better route.
        private const int DistanceThreshold = 3000;

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Returns the shortest route within the time limit. (Duration <= 300s , Length difference > 3000m)
        /// </summary>
        /// <param name="transportType">Type of transport. Car or bike.</param>
        /// <param name="addresses"></param>
        /// <exception cref="AddressLaunderingException"></exception>
        /// <exception cref="AddressCoordinatesException"></exception>
        /// <exception cref="RouteInformationException"></exception>
        /// <returns></returns>
        public RouteInformation GetRoute(DriveReportTransportType transportType, IEnumerable<Address> addresses)
        {
            if (addresses == null || !addresses.Any())
            {
                return null;
            }
            var addressesList = addresses.ToList();

            List<Coordinates> routeCoordinates = new List<Coordinates>();
            AddressCoordinates coordService = new AddressCoordinates();
            SeptimaRouter septimaService = new SeptimaRouter();
            
            var origin = addressesList[0];
            var destination = addressesList[addressesList.Count - 1];

            addressesList.Remove(origin);
            addressesList.Remove((destination));

            if (String.IsNullOrEmpty(origin.Longitude))
            {
                routeCoordinates.Add(coordService.GetCoordinates(origin, Coordinates.CoordinatesType.Origin));
            }
            else
            {
                routeCoordinates.Add(new Coordinates()
                {
                    Longitude = origin.Longitude,
                    Latitude = origin.Latitude,
                    Type = Coordinates.CoordinatesType.Origin
                });
            }

            foreach (var address in addressesList)
            {
                if (String.IsNullOrEmpty(address.Longitude))
                {
                    routeCoordinates.Add(coordService.GetCoordinates(address,
                        Coordinates.CoordinatesType.Via));
                }
                else
                {
                    routeCoordinates.Add(new Coordinates()
                    {
                        Longitude = address.Longitude,
                        Latitude = address.Latitude,
                        Type = Coordinates.CoordinatesType.Via
                    });
                }
            }

            if (String.IsNullOrEmpty(destination.Longitude))
            {
                routeCoordinates.Add(coordService.GetCoordinates(destination, Coordinates.CoordinatesType.Destination));
            }
            else
            {
                routeCoordinates.Add(new Coordinates()
                {
                    Longitude = destination.Longitude,
                    Latitude = destination.Latitude,
                    Type = Coordinates.CoordinatesType.Destination
                });
            }

            try
            {
                List<RouteInformation> routes =
                    septimaService.GetRoute(transportType, routeCoordinates).OrderBy(x => x.Duration).ToList();
                RouteInformation bestRoute = routes[0];

                foreach (var route in routes)
                {
                    // Iterate all found routes and find the best one.
                    // A route is considered better than the previous best route if the difference in distance is greater than DistanceThreshold
                    // and if the difference in time in seconds is greater than TimeTreshold.
                    bool betterRoute = (route.Duration - bestRoute.Duration <= TimeThreshold) && (bestRoute.Length - route.Length > DistanceThreshold);
                    if (betterRoute)
                    {
                        bestRoute = route;
                    }
                }

                // Divide by 1000 to get it in kilometers.
                bestRoute.Length /= 1000;
                return bestRoute;
            }
            catch (AddressCoordinatesException e)
            {
                Logger.Error("Exception when getting route information", e);
            }
            return null;
        }
    }
}
