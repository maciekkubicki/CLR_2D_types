
declare @point [dbo].[Point];
declare @point1 [dbo].[Point];
declare @point2 [dbo].[Point];
declare @point3 [dbo].[Point];
declare @point4 [dbo].[Point];
set @point ='(0,0)';
--set @point ='(0,0';--exception
set @point1 = [dbo].[MakePoint](0,2);
set @point2 = [dbo].[MakePoint](2,2);
set @point3 = [dbo].[MakePoint](2,0);
set @point4 = [dbo].[MakePoint](1,4);
select @point.GetX();
select @point.GetY();
select @point.asString();

declare @seg [dbo].[Segment];
set @seg = ''+@point1.asString()+';'+@point4.asString();
select @seg.asString();

declare @stra [dbo].[Straight];
declare @stra2 [dbo].[Straight];
set @stra = '(1.2,4)'
select @stra.asString();
--set @stra2 = '(0,0)'; --exception 

--select @point.asString()+';'+@point1.asString()+';'+@point2.asString()+';'+@point2.asString(); --exception
declare @poly [dbo].[Polygon]
set @poly=''+@point.asString()+';'+@point1.asString()+';'+@point2.asString()+';'+@point3.asString();
select @poly.NumberOfVerts();
select @poly.asString();
select [dbo].PointInPolygon(@poly,'(0,0)');
select [dbo].PointInPolygon(@poly,@point);
select [dbo].PointInPolygon(@poly,'(2.1,0)');
select [dbo].PointInPolygon(@poly,'(3,2)');
select [dbo].PointInPolygon(@poly,'(2,3)');
select [dbo].PointInPolygon(@poly,'(-2,2.1)');