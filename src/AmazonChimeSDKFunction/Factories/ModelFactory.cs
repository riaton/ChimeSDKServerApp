﻿using Google.Protobuf;
using ChimeApp.Domain;

namespace ChimeApp.Factories
{
    public class ModelFactory
    {
        public static (bool validateOk, T? model) CreateModel<T>(T body) where T : IMessage
        {
            var result = false;
            T? model = default;

            try
            {
                if (body is null)
                {
                    Console.WriteLine("model deserialize error");
                }
                else
                {
                    model = body;
                    var results = BasicValidator.Validate(body);
                    if (results.Count == 0)
                    {
                        result = true;
                    }
                    else
                    {
                        Console.WriteLine($"Validation error body = {body}");
                        foreach (var r in results)
                        {
                            Console.WriteLine($"Validation error reason: {r}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred at modelfactory, " + e);
            }

            return (result, model);
        }
    }
}
