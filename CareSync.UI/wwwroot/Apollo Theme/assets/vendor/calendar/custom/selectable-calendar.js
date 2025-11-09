document.addEventListener("DOMContentLoaded", function () {
	var calendarEl = document.getElementById("selectableCalendar");

	var calendar = new FullCalendar.Calendar(calendarEl, {
		headerToolbar: {
			left: "prev,next today",
			center: "title",
			right: "dayGridMonth,timeGridWeek,timeGridDay",
		},
		initialDate: "2022-10-12",
		navLinks: true, // can click day/week names to navigate views
		selectable: true,
		selectMirror: true,
		select: function (arg) {
			var title = prompt("Event Title:");
			if (title) {
				calendar.addEvent({
					title: title,
					start: arg.start,
					end: arg.end,
					allDay: arg.allDay,
				});
			}
			calendar.unselect();
		},
		eventClick: function (arg) {
			if (confirm("Are you sure you want to delete this event?")) {
				arg.event.remove();
			}
		},
		editable: true,
		dayMaxEvents: true, // allow "more" link when too many events
		events: [
			{
				title: "All Day Event",
				start: "2022-10-01",
				color: "#a7874b",
			},
			{
				title: "Long Event",
				start: "2022-10-07",
				end: "2022-10-10",
				color: "#627aad",
			},
			{
				groupId: 999,
				title: "Birthday",
				start: "2022-10-09T16:00:00",
				color: "#35508a",
			},
			{
				groupId: 999,
				title: "Birthday",
				start: "2022-10-16T16:00:00",
				color: "#3b5999",
			},
			{
				title: "Conference",
				start: "2022-10-11",
				end: "2022-10-13",
				color: "#4f6aa3",
			},
			{
				title: "Meeting",
				start: "2022-10-12T10:30:00",
				end: "2022-10-12T12:30:00",
				color: "#627aad",
			},
			{
				title: "Lunch",
				start: "2022-10-12T12:00:00",
				color: "#899bc2",
			},
			{
				title: "Meeting",
				start: "2022-10-12T14:30:00",
				color: "#768bb8",
			},
			{
				title: "Interview",
				start: "2022-10-12T17:30:00",
				color: "#9daccc",
			},
			{
				title: "Meeting",
				start: "2022-10-12T20:00:00",
			},
			{
				title: "Birthday",
				start: "2022-10-13T07:00:00",
				color: "#b1bdd6",
			},
			{
				title: "Click for Google",
				url: "http://google.com/",
				start: "2022-10-28",
				color: "#35508a",
			},
		],
	});

	calendar.render();
});
