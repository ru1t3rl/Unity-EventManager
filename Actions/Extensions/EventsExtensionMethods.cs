using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Ru1t3rl.Events;

namespace Ru1t3rl.Extensions.Events
{
    public static class ActionExtension
    {
        public static string PrintList(this ParameterInfo[] parameters)
        {
            string parametersString = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                parametersString += $"{parameters[i].ParameterType.ToString().Split('.')[parameters[i].ParameterType.ToString().Split('.').Length - 1]}" +
                                    $" {parameters[i].Name}" +
                                    $"{(i != parameters.Length - 1 ? ", " : "")}";
            }
            return parametersString;
        }

        public static string[] ToNames(this Component[] components) => components.Select(component => component.GetType().ToName()).ToArray();

        public static string ToName(this System.Type type) => type.ToString().Split('.')[type.ToString().Split('.').Length - 1];

        public static string Print(this MethodInfo method) => $"{method.Name}({method.GetParameters().PrintList()})";

        public static string[][] ToMethods(this Component[] components, BindingFlags bindingFlags)
        {
            return components.Select(component => component.GetType().GetMethods(bindingFlags).Select(method => method.ToString()).ToArray()).ToArray();
        }

        public static List<System.Action> ToActionList(this List<ActionField> actionFields)
        {
            return actionFields.Select(action => (System.Action)action).ToList();
        }

        public static string[] ToNames(this MethodInfo[] methods) => methods.Select(method => method.Print()).ToArray();
    }
}