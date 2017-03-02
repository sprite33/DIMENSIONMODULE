/*
 * Created by SharpDevelop.
 * User: hcha
 * Date: 10/6/2014
 * Time: 1:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace DIMENSIONMODULE
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("7DF52626-A501-4958-B383-DA39062FB1FD")]
	public partial class ThisApplication
	{
		private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}
		
public void IdentifyGeometry()
		{
			Document doc = this.ActiveUIDocument.Document;
            UIDocument uidoc = this.ActiveUIDocument;
            
            Options opt = doc.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;
            
            ReferenceArray referenceArray = new ReferenceArray();
            
		   ISelectionFilter horwallfilter = new HorWallSelectionFilter();
		            
		 	foreach  (Element wall in uidoc.Selection.PickElementsByRectangle())
		  { 
            	
       		
	            GeometryElement geo = wall.get_Geometry( opt );
	            
	            
	            foreach( GeometryObject geoObject in geo )
				    {
				      GeometryInstance instance = geoObject as GeometryInstance;				 
				      
				        foreach( GeometryObject instObj in instance.SymbolGeometry )
				        {
				          if( instObj is Curve )
				          {
				          	TaskDialog.Show("11","Symbol Geometry is Curve");
				          }
				          else if( instObj is PolyLine )
				          {
				          	TaskDialog.Show("11","Symbol Geometry is Polyline");
				          }
				          else if( instObj is Solid )
				          {
				          	TaskDialog.Show("11","Symbol Geometry is Solid");
				          }
				        }
				      
				    }    	
 			}     	
 			

		}
		
		public void CreateLegendComponentDimension()
		{
			Document doc = this.ActiveUIDocument.Document;
            UIDocument uidoc = this.ActiveUIDocument;
            
            Options opt = doc.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;
            
            IList<Face> hfaces = new List<Face>();
            IList<Face> vfaces = new List<Face>();
            ReferenceArray referenceArray_h = new ReferenceArray();
            ReferenceArray referenceArray_v = new ReferenceArray();
            
		   ISelectionFilter horwallfilter = new HorWallSelectionFilter();
		            
		   Element wall =  doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element));
		  
            	
       		
	            GeometryElement geo = wall.get_Geometry( opt );	            
	            
	            foreach( GeometryObject geoObject in geo )
				    {
				      GeometryInstance instance = geoObject as GeometryInstance;				 
				      
				        foreach( GeometryObject instObj in instance.SymbolGeometry )
				        {
				          Solid solid = instObj as Solid;
				            	foreach ( Face face in solid.Faces)	
				            	{
				            		
				            		if (face.ComputeNormal(new UV(0,0)).Y == 1 || face.ComputeNormal(new UV(0,0)).Y == -1 )
				            		{hfaces.Add(face);}
				            		if (face.ComputeNormal(new UV(0,0)).X == 1 || face.ComputeNormal(new UV(0,0)).X == -1 )
				            		{vfaces.Add(face); }
				            	}
				        }
				      
				    } 
	            
	            hfaces = hfaces.OrderBy(l => l.Evaluate(new UV(0,0)).Y).ToList();
	            vfaces = vfaces.OrderBy(l => l.Evaluate(new UV(0,0)).X).ToList();
	            
	            for (int i = 0 ; i < hfaces.Count ; i++  ) { 
	            	if ( i == hfaces.Count-1)
	            	{referenceArray_h.Append( hfaces[i].Reference );}
	            	else if (hfaces[i+1].Evaluate(new UV(0,0)).Y - hfaces[i].Evaluate(new UV(0,0)).Y > 0.0001 )
	            	{referenceArray_h.Append( hfaces[i].Reference );} 
	            }
	            
	            for (int i = 0 ; i < vfaces.Count ; i++  ) { 
	            	
	            	if ( i == vfaces.Count-1)
	            	{referenceArray_v.Append( vfaces[i].Reference );}
	            	else if ( vfaces[i+1].Evaluate(new UV(0,0)).X - vfaces[i].Evaluate(new UV(0,0)).X  > 0.0001 )
	            	{
	            	referenceArray_v.Append( vfaces[i].Reference );} 
	            }
	             
 			

XYZ pnt = uidoc.Selection.PickPoint(ObjectSnapTypes.None);

 	try
      { 
        XYZ p1 = pnt + new XYZ(0, 1, 0);
        XYZ p2 = pnt + new XYZ(0, -1, 0);
        XYZ p3 = pnt + new XYZ(1, 0, 0);
        XYZ p4 = pnt + new XYZ(-1, 0, 0);

        Line newLine_h = Line.CreateBound(p1,p2); 
        Line newLine_v = Line.CreateBound(p3,p4); 
        	
        	using (Transaction t = new Transaction(doc,"dim"))
    		{
        	t.Start();
        	Dimension newDimension_h = doc.Create.NewDimension( doc.ActiveView, newLine_h, referenceArray_h); 
        	Dimension newDimension_v = doc.Create.NewDimension( doc.ActiveView, newLine_v, referenceArray_v); 
        	t.Commit();
   		 	}
    } 
      // catch the exceptions 
      catch 
      { 
      	TaskDialog.Show("11","script error ask Hongseok");
          
      } 		 	
 			

		}
		
		public void Create_V_Mullion_Dimension()
		{
			Document doc = this.ActiveUIDocument.Document;
            UIDocument uidoc = this.ActiveUIDocument;
            
            Options opt = doc.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;
            
            IList<Face> vfaces = new List<Face>();
            ReferenceArray referenceArray_v = new ReferenceArray();
            
		   ISelectionFilter mullionfilter = new MullionSelectionFilter();
		   
		   foreach (Element mul in uidoc.Selection.PickElementsByRectangle(mullionfilter))
		   {
	       GeometryElement geo = mul.get_Geometry( opt );	            
	            
	            foreach( GeometryObject geoObject in geo )
				    {
				      GeometryInstance instance = geoObject as GeometryInstance;				 
				      
				        foreach( GeometryObject instObj in instance.SymbolGeometry )
				        {
				          Solid solid = instObj as Solid;
				            	foreach ( Face face in solid.Faces)	
				            	{
				            		
				            		if (face.ComputeNormal(new UV(0,0)).X == 1 || face.ComputeNormal(new UV(0,0)).X == -1 )
				            		{referenceArray_v.Append(face.Reference); }
				            	}
				        }
				      
				    }

		   }
	            
	            
 			

XYZ pnt = uidoc.Selection.PickPoint(ObjectSnapTypes.None);

 	try
      { 
        XYZ p1 = pnt + new XYZ(0, 0, 1);
        XYZ p2 = pnt + new XYZ(0, 0, -1);
        XYZ p3 = pnt + new XYZ(1, 0, 0);
        XYZ p4 = pnt + new XYZ(-1, 0, 0);

        Line newLine_v = Line.CreateBound(p3,p4); 
        	
        	using (Transaction t = new Transaction(doc,"dim"))
    		{
        	t.Start();
        	Dimension newDimension_v = doc.Create.NewDimension( doc.ActiveView, newLine_v, referenceArray_v); 
        	t.Commit();
   		 	}
    } 
      // catch the exceptions 
      catch  
      { 
      	TaskDialog.Show("11","script error ask Hongseok");
          
      } 	
      
		}
		
				public void Create_Mullion_Dimension()
		{
			Document doc = this.ActiveUIDocument.Document;
            UIDocument uidoc = this.ActiveUIDocument;
            
            Transaction t = new Transaction(doc,"script");
            t.Start();
            if ( uidoc.ActiveView.SketchPlane == null)
            {
            Plane plane = new Plane(uidoc.ActiveView.ViewDirection, uidoc.ActiveView.Origin);
        	SketchPlane sp = SketchPlane.Create(doc, plane);
        	uidoc.ActiveView.SketchPlane = sp;
        	uidoc.ActiveView.HideActiveWorkPlane();
            }
        	t.Commit();
            
            Options opt = doc.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;
            
            IList<Face> hfaces = new List<Face>();
            IList<Face> vfaces = new List<Face>();
            ReferenceArray referenceArray_h = new ReferenceArray();
            ReferenceArray referenceArray_v = new ReferenceArray();
            
		   ISelectionFilter mullionfilter = new MullionSelectionFilter();
		   
		  IList<Element> selec = uidoc.Selection.PickElementsByRectangle(mullionfilter);
		   foreach (Element e in uidoc.Selection.PickElementsByRectangle(mullionfilter) )
		   { selec.Add(e);}
		   
		   foreach (Mullion mul in selec)
		   { 
		   	
		   	if (mul.HandOrientation.Z == 1 || mul.HandOrientation.Z == -1)
		   		
		   	{
		   	
	       GeometryElement geo = mul.get_Geometry( opt );	            
	            
	            foreach( GeometryObject geoObject in geo )
				    {
				      GeometryInstance instance = geoObject as GeometryInstance;				 
				      
				        foreach( GeometryObject instObj in instance.SymbolGeometry )
				        {
				          Solid solid = instObj as Solid;
				            	foreach ( Face face in solid.Faces)	
				            	{
				            		
				            		if (face.ComputeNormal(new UV(0,0)).X == 1 || face.ComputeNormal(new UV(0,0)).X == -1 )
				            		{referenceArray_h.Append( face.Reference);}
				            	}
				        }
				      
				    }
		   	}
		   	
		   	if (mul.HandOrientation.X == 1 || mul.HandOrientation.X == -1 || mul.HandOrientation.Y == -1  || mul.HandOrientation.Y == 1)
		   		
		   	{
		   	
	       GeometryElement geo = mul.get_Geometry( opt );	            
	            
	            foreach( GeometryObject geoObject in geo )
				    {
				      GeometryInstance instance = geoObject as GeometryInstance;				 
				      
				        foreach( GeometryObject instObj in instance.SymbolGeometry )
				        {
				          Solid solid = instObj as Solid;
				            	foreach ( Face face in solid.Faces)	
				            	{
				            		
				            		if (face.ComputeNormal(new UV(0,0)).X == 1 || face.ComputeNormal(new UV(0,0)).X == -1 )
				            		{referenceArray_v.Append( face.Reference);}
				            	}
				        }
				      
				    }
		   	}

		   }
	             
 			

XYZ pnt = uidoc.Selection.PickPoint(ObjectSnapTypes.None);

 	try
      { 
        XYZ up = pnt + uidoc.ActiveView.UpDirection;

        XYZ right = pnt + uidoc.ActiveView.RightDirection;

		
        Line newLine_v = Line.CreateBound(pnt, right); 
        Line newLine_h = Line.CreateBound(pnt,up); 
        	
        	using (Transaction tr = new Transaction(doc,"dim"))
    		{
        	tr.Start();
        	Dimension newDimension_h = doc.Create.NewDimension( doc.ActiveView, newLine_h, referenceArray_h);
			Dimension newDimension_v = doc.Create.NewDimension( doc.ActiveView, newLine_v, referenceArray_v);   
			
        	uidoc.ActiveView.HideActiveWorkPlane();			
        	tr.Commit();
   		 	}
    } 
      // catch the exceptions 
      catch  
      { 
      	TaskDialog.Show("11","script error ask Hongseok");
          
      } 	
      
		}
		
public void CreateWallDimensions_Horizontal()
		{
			Document doc = this.ActiveUIDocument.Document;
            UIDocument uidoc = this.ActiveUIDocument;
            
            Options opt = doc.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;
            
            ReferenceArray referenceArray = new ReferenceArray();
            
		   ISelectionFilter horwallfilter = new HorWallSelectionFilter();
		            
		 	foreach  (Element wall in uidoc.Selection.PickElementsByRectangle())
		  { 
            	
       		
	            GeometryElement geo = wall.get_Geometry( opt );
	            foreach (GeometryObject objects in geo)
	            {
	            	Solid solid = objects as Solid;
	            	foreach ( Face face in solid.Faces)	
	            	{
	            		
	            		if (face.ComputeNormal(new UV(0,0)).Y == 1 || face.ComputeNormal(new UV(0,0)).Y == -1 )
	            		{referenceArray.Append( face.Reference ); }
	            	}
	            }     	
 			}	
      
 	
            XYZ pnt = uidoc.Selection.PickPoint(ObjectSnapTypes.None);
 	try
      { 
     

        XYZ p1 = pnt + new XYZ(0, 1, 0);
        XYZ p2 = pnt + new XYZ(0, -1, 0);

        Line newLine2 = Line.CreateBound(p1,p2); 
        	
        	using (Transaction t = new Transaction(doc,"dim"))
    		{
        	t.Start();
        	Dimension newDimension = doc.Create.NewDimension( doc.ActiveView, newLine2, referenceArray); 
        	t.Commit();
   		 	}
    } 
      // catch the exceptions 
      catch 
      { 
      	TaskDialog.Show("11","script error ask Hongseok");
          
      } 
  
                                    
		}
		
		
public void CreateWallDimensions_Vertical()
		{
			Document doc = this.ActiveUIDocument.Document;
            UIDocument uidoc = this.ActiveUIDocument;
            
            Options opt = doc.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;
            
            ReferenceArray referenceArray = new ReferenceArray();
            
   ISelectionFilter verwallfilter = new VerWallSelectionFilter();
            
 	foreach  (Wall wall in uidoc.Selection.PickElementsByRectangle(verwallfilter))
  { 
            	
       		
            GeometryElement geo = wall.get_Geometry( opt );
            foreach (GeometryObject objects in geo)
            {
            	Solid solid = objects as Solid;
            	foreach ( Face face in solid.Faces)	
            	{
            		
            		if (face.ComputeNormal(new UV(0,0)).X == 1 || face.ComputeNormal(new UV(0,0)).X == -1 )
            		{referenceArray.Append( face.Reference ); }
            	}
            }     	
 	}
      
 	
            XYZ pnt = uidoc.Selection.PickPoint(ObjectSnapTypes.None);
 	try
      { 
     

        XYZ p1 = pnt + new XYZ(1, 0, 0);
        XYZ p2 = pnt + new XYZ(-1, 0, 0);

        Line newLine2 = Line.CreateBound(p1,p2); 
        	
        	using (Transaction t = new Transaction(doc,"dim"))
    		{
        	t.Start();
        	Dimension newDimension = doc.Create.NewDimension( doc.ActiveView, newLine2, referenceArray); 
        	t.Commit();
   		 	}
    } 
      // catch the exceptions 
      catch  
      { 
      	TaskDialog.Show("11","script error ask Hongseok");
          
      } 
  
                                    
		}
		
private ReferenceArray SortRA ( ReferenceArray ra )
{
	IList<Reference> list = null;
	ReferenceArray ra2 = null ;
	
	foreach ( Reference r in ra) {list.Add(r);}
	foreach ( Reference r2 in list) { ra2.Append(r2); }
	             
	return ra2;
}

		#region Revit Macros generated code
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
	}
	
		public class VerWallSelectionFilter : ISelectionFilter
		{
    		public bool AllowElement(Element element)
    			{
       
    			if (element.Category.Name == "Walls" && ((LocationCurve)element.Location).Curve.GetEndPoint(0).X == ((LocationCurve)element.Location).Curve.GetEndPoint(1).X )
       		 		{
           		 return true;
        			}
       		 return false;
    			}

    		public bool AllowReference(Reference refer, XYZ point)
    		{return false;}
		}
		
		public class MullionSelectionFilter : ISelectionFilter
		{
    		public bool AllowElement(Element element)
    			{
       
    			if (element.Category.Name == "Curtain Wall Mullions"  )
       		 		{
           		 return true;
        			}
       		 return false;
    			}

    		public bool AllowReference(Reference refer, XYZ point)
    		{return false;}
		}
		
		public class HorWallSelectionFilter : ISelectionFilter
		{
    		public bool AllowElement(Element element)
    			{
       
    			if (element.Category.Name == "Walls" && ((LocationCurve)element.Location).Curve.GetEndPoint(0).Y == ((LocationCurve)element.Location).Curve.GetEndPoint(1).Y )
       		 		{
           		 return true;
        			}
       		 return false;
    			}

    		public bool AllowReference(Reference refer, XYZ point)
    		{return false;}
		}
}