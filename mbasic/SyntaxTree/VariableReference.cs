/*******************************************************************************
    Copyright 2006 Michael Welch
    
    This file is part of MBasic99.

    MBasic99 is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    MBasic99 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MBasic99; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*******************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace mbasic.SyntaxTree
{
    internal class LocationReference : Expression
    {
        Location location;
        public LocationReference(Location loc, LineId line)
            : base(line)
        {
            this.location = loc;
        }

        public Location Location { get { return location; } }

        public override void Emit(ILGenerator gen)
        {
            location.EmitLoad(gen, locals);
            //if (index < 255)
            //{
            //    switch (index)
            //    {
            //        case 0:
            //            gen.Emit(OpCodes.Ldloc_0);
            //            break;
            //        case 1:
            //            gen.Emit(OpCodes.Ldloc_1);
            //            break;
            //        case 2:
            //            gen.Emit(OpCodes.Ldloc_2);
            //            break;
            //        case 3:
            //            gen.Emit(OpCodes.Ldloc_3);
            //            break;
            //        default:
            //            gen.Emit(OpCodes.Ldloc_S, locals[index]);
            //            break;
                        
            //    }
            //}
            //else
            //{
            //    gen.Emit(OpCodes.Ldloc, locals[index]);
            //}
        }

        public override BasicType GetBasicType()
        {
            location.ConstrainType(symbols);
            return location.BasicType;
        }
    }
}
