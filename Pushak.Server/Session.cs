﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Pushak.Server
{
    public class Session
    {
        readonly ConcurrentQueue<string> buffer = new ConcurrentQueue<string>();

        public string Key { get; private set; }
        public DateTime CreatedOnUtc { get; private set; }
        public State State { get; set; }

        public Session(string key, DateTime createdOnUtc)
        {
            this.Key = key;
            this.CreatedOnUtc = createdOnUtc;
        }

        public void WriteBuffer(string s)
        {
            this.buffer.Enqueue("Pushak.Server:: " + s);
        }

        public string ReadBuffer()
        {
            var ss = new List<string>();
            string s;
            while (buffer.TryDequeue(out s)) { ss.Add(s); }
            return String.Join(Environment.NewLine, ss);
        }
    }
}