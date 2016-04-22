	
	function rev_loaCharts(){
	
		//#
		//# Data Pie Chart
		//#			
		var data_pie = new google.visualization.DataTable();
			data_pie.addColumn('string', 'Task');
			data_pie.addColumn('number', 'Hours per Day');
			data_pie.addRows(5);
			data_pie.setValue(0, 0, 'Work');
			data_pie.setValue(0, 1, 11);
			data_pie.setValue(1, 0, 'Eat');
			data_pie.setValue(1, 1, 2);
			data_pie.setValue(2, 0, 'Commute');
			data_pie.setValue(2, 1, 2);
			data_pie.setValue(3, 0, 'Watch TV');
			data_pie.setValue(3, 1, 2);
			data_pie.setValue(4, 0, 'Sleep');
			data_pie.setValue(4, 1, 7);
	
		if ($('#chart_pie').size()){
			var chart_pie = new google.visualization.PieChart(document.getElementById('chart_pie'));
			chart_pie.draw(data_pie, { chartArea:{left:0,top:0,width:"100%",height:"90%"}, backgroundColor:'transparent', width: 215, height: 180,  colors:['#fa9c20','#d66f15', '#ad5326', '#704c44', '#424a4f'] });
		}
		
	
		//#
		//# Data Columns
		//#
		var data_column = new google.visualization.DataTable();
		data_column.addColumn('string', 'Year');
		
		data_column.addColumn('number', 'Sales');
		data_column.addColumn('number', 'Expenses');
		data_column.addColumn('number', 'Employment');
		data_column.addColumn('number', 'Extra');
		
		data_column.addRows(4);
		data_column.setValue(0, 0, '2004');
		data_column.setValue(0, 1, 1000);
		data_column.setValue(0, 2, 400);
		data_column.setValue(0, 3, 800);
		data_column.setValue(0, 4, 430);
		
		data_column.setValue(1, 0, '2005');
		data_column.setValue(1, 1, 1170);
		data_column.setValue(1, 2, 460);
		data_column.setValue(1, 3, 920);
		data_column.setValue(1, 4, 234);
		
		data_column.setValue(2, 0, '2006');
		data_column.setValue(2, 1, 660);
		data_column.setValue(2, 2, 1120);
		data_column.setValue(2, 3, 230);
		data_column.setValue(2, 4, 632);
		
		data_column.setValue(3, 0, '2007');
		data_column.setValue(3, 1, 1030);
		data_column.setValue(3, 2, 540);
		data_column.setValue(3, 3, 840);
		data_column.setValue(3, 4, 340);

		if ($('#chart_column').size()){
			var chart_column = new google.visualization.ColumnChart(document.getElementById('chart_column'));
			chart_column.draw(data_column, { backgroundColor:'transparent', chartArea:{left:0,top:0,width:"85%",height:"85%"}, width: 900, height: 320, title: 'Company Performance', colors: ['#fa9c20','#d66f15', '#ad5326', '#704c44', '#424a4f'] });
		}
	
  
		//#
		//# Data Area Chart
		//#	
		var data_area = new google.visualization.DataTable();
		data_area.addColumn('string', 'Year');
		data_area.addColumn('number', 'Sales');
		data_area.addColumn('number', 'Expenses');
		data_area.addColumn('number', 'Employees');
		data_area.addRows([
		  ['2004', 1000, 400, 200],
		  ['2005', 1170, 460, 120],
		  ['2006', 660, 1120, 880],
		  ['2007', 1030, 540, 490],
		  ['2008', 1030, 540, 900]
		]); 

		if ($('#chart_area').size()){
			var chart_area = new google.visualization.AreaChart(document.getElementById('chart_area'));
			chart_area.draw(data_area, { backgroundColor:'transparent', chartArea:{left:0,top:0,width:"85%",height:"65%"}, width: 215, height: 180,  colors: ['#fa9c20','#704c44', '#ad5326' ] });
		}
		
		
  
		//#
		//# Data Area Line
		//#			  
		
		var data_line = new google.visualization.DataTable();
		data_line.addColumn('string', 'Year');
		data_line.addColumn('number', 'Sales');
		data_line.addColumn('number', 'Expenses');
		data_line.addColumn('number', 'Employees');
		data_line.addRows(4);
		data_line.setValue(0, 0, '2004');
		data_line.setValue(0, 1, 1000);
		data_line.setValue(0, 2, 400);
		data_line.setValue(0, 3, 490);
		data_line.setValue(1, 0, '2005'); 
		data_line.setValue(1, 1, 1170);
		data_line.setValue(1, 2, 460);
		data_line.setValue(1, 3, 960);
		data_line.setValue(2, 0, '2006');
		data_line.setValue(2, 1, 860);
		data_line.setValue(2, 2, 580);
		data_line.setValue(2, 3, 980);
		data_line.setValue(3, 0, '2007');
		data_line.setValue(3, 1, 1030);
		data_line.setValue(3, 2, 540);
		data_line.setValue(3, 3, 240);

		if ($('#chart_line').size()){
			var chart_line = new google.visualization.LineChart(document.getElementById('chart_line'));
			chart_line.draw(data_line, {backgroundColor:'transparent', width: 215, height: 180, chartArea:{left:0,top:0,width:"85%",height:"65%"}, colors: ['#fa9c20','#704c44', '#ad5326' ] });
		}
		
		
		
		//#
		//# Data Stick
		//#	
		/*
		var data_stick = google.visualization.arrayToDataTable([
		   ['Mon',20,28,38,45],
		   ['Tues',31,38,55,66],
		   ['Wed',50,55,77,80],
		   ['Thurs',50,77,66,77],
		   ['Fri',15,66,22,68]
		 ], true);

		var chart_stick = new google.visualization.CandlestickChart(document.getElementById('chart_stick'));
		chart_stick.draw(data_stick, { backgroundColor:'transparent', width: 900, height: 320, title:'Company Work', colors: ['#fa9c20']}); 
		*/
		
		
		
		//#
		//# Data Bar
		//#	
		
		var data_bar = new google.visualization.DataTable();
		data_bar.addColumn('string', 'Year');
		data_bar.addColumn('number', 'Sales');
		data_bar.addColumn('number', 'Expenses');
		data_bar.addRows(4);
		data_bar.setValue(0, 0, '2004');
		data_bar.setValue(0, 1, 1000);
		data_bar.setValue(0, 2, 400);
		data_bar.setValue(1, 0, '2005');
		data_bar.setValue(1, 1, 1170);
		data_bar.setValue(1, 2, 460);
		data_bar.setValue(2, 0, '2006');
		data_bar.setValue(2, 1, 660);
		data_bar.setValue(2, 2, 1120);
		data_bar.setValue(3, 0, '2007');
		data_bar.setValue(3, 1, 1030);
		data_bar.setValue(3, 2, 540);

		if ($('#chart_bar').size()){
			var chart_bar = new google.visualization.BarChart(document.getElementById('chart_bar'));
			chart_bar.draw(data_bar, { backgroundColor:'transparent', chartArea:{left:20,top:0,width:"75%",height:"65%"}, width: 215, height: 180, colors: ['#fa9c20','#704c44', '#ad5326' ]  });
		}
		
	}