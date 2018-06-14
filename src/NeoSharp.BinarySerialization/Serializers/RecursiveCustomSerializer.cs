﻿using System;
using System.IO;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class RecursiveCustomSerializer : IBinaryCustomSerialization
    {
        /// <summary>
        /// Type
        /// </summary>
        private readonly Type Type;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        public RecursiveCustomSerializer(Type type)
        {
            Type = type;
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            return serializer.Serialize(value, writer);
        }

        public object Deserialize(IBinaryDeserializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            return deserializer.Deserialize(reader, Type);
        }
    }
}
