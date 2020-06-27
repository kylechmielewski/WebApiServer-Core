﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Helpers
{
    public class ShapedEntity
    {
        public Guid Id { get; set; }
        public Entity Entity { get; set; }

        public ShapedEntity()
        {
            Entity = new Entity();
        }
    }
}