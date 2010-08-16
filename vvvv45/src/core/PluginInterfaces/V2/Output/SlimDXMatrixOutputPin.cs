﻿using System;
using System.Runtime.InteropServices;
using SlimDX;
using VVVV.PluginInterfaces.V1;

namespace VVVV.PluginInterfaces.V2.Output
{
	public class SlimDXMatrixOutputPin : Pin<Matrix>, IPinUpdater
	{
		protected ITransformOut FTransformOut;
		protected float[] FData;
		protected int FSliceCount;
		
		public SlimDXMatrixOutputPin(IPluginHost host, OutputAttribute attribute)
		{
			host.CreateTransformOutput(attribute.Name, (TSliceMode)attribute.SliceMode, (TPinVisibility)attribute.Visibility, out FTransformOut);
			
			FTransformOut.SetPinUpdater(this);
			
			SliceCount = 1;
		}
		
		public override IPluginIO PluginIO 
		{
			get
			{
				return FTransformOut;
			}
		}
		
		public override int SliceCount 
		{
			get 
			{
				return FSliceCount;
			}
			set 
			{
				if (FSliceCount != value)
					FData = new float[value * 16];
				
				FSliceCount = value;
				FTransformOut.SliceCount = value;
			}
		}
		
		unsafe public override Matrix this[int index] 
		{
			get 
			{
				throw new NotImplementedException();
			}
			set 
			{
				fixed (float* ptr = FData)
				{
					((Matrix*)ptr)[index % FSliceCount] = value;
				}
			}
		}
		
		unsafe public override void Update()
		{
			base.Update();
			
			float* destination;
			FTransformOut.GetMatrixPointer(out destination);
			
			Marshal.Copy(FData, 0, new IntPtr(destination), FData.Length);
		}
	}
}
