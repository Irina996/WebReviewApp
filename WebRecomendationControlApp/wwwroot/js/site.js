// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function () {
    $("#send-comment-button").click(function (e) {
        e.preventDefault();
        var reviewId = $("#ReviewId").val();
        var commentText = $("#CommentText").val();
        $("#CommentText").val("");

        var posting = $.post("/Comment/Create", { ReviewId: reviewId, CommentText: commentText });
        posting.done(function (data) {
            $("#commentList").load("/Comment/GetComments", { ReviewId: reviewId });
        });
	});
});

function onChangeAdd() {
	let container = document.getElementById("tagsContainer");
	let fields = container.getElementsByTagName("input");

	let i = 0;
	var empty = 0;
	while (fields[i] != undefined) {
		if (fields[i].value == "") {
			empty = empty + 1;
		}
		i = i + 1;
	}
	if (empty > 1) {
		DeleteEmptyTags();
		empty = 0;
	}
	if (empty == 0) {
		let fieldCount = container.getElementsByTagName("input").length;
		let nextFieldId = fieldCount + 1;

		let field = document.createElement("input");
		field.setAttribute("id", "Tag[" + nextFieldId + "]");
		field.setAttribute("name", "ReviewTags");
		field.setAttribute("type", "text");
		field.setAttribute("onchange", "onChangeAdd()");
		field.setAttribute("autocomplete", "on");

		container.appendChild(field);
	}
}

function DeleteEmptyTags() {
	let container = document.getElementById("tagsContainer");
	let fields = container.getElementsByTagName("input");
	let fieldCount = container.getElementsByTagName("input").length;
	var i = 0;
	while (i <= fieldCount && fields[i] != undefined) {
		if (fields[i].value == "") {
			let k = i + 1;
			while (fields[k] != undefined && fields[k].value == "") {
				k = k + 1;
			}
			let j = i;
			while (fields[k] != undefined) {
				fields[j].value = fields[k].value;
				fields[k].value = "";
				j = j + 1;
				k = k + 1;
			}
		}
		i = i + 1;
	}
	i = 0;
	while (fields[i] != undefined) {
		if (fields[i].value == "") {
			let removedEl = container.removeChild(fields[i])
		}
		else {
			i = i + 1;
		}
	}
}

function ChangeLikeStatus(id, uId) {
	var like = document.getElementById("like");
	if (like.classList.contains("fa")) {
		like.classList.remove("fa");
		like.classList.add("far");
	}
	else {
		like.classList.remove("far");
		like.classList.add("fa");
    }
	var reseting = $.post("/Like/ResetLike", { reviewId: id, userId: uId });
}

function ChangeStarStatus(starId, reviewId, userId) {
	var id = parseInt(starId.slice(4));
	var i = 1;
	for (; i <= id; i++) {
		var star = document.getElementById("star" + i);
		if (star.classList.contains("far")) {
			star.classList.remove("far");
			star.classList.add("fa");
		}
	}
	for (; i <= 5; i++) {
		var star = document.getElementById("star" + i);
		if (star.classList.contains("fa")) {
			star.classList.remove("fa");
			star.classList.add("far");
		}
	}
	var reseting = $.post("/Rate/ResetRate", { starCount: id, reviewId: reviewId, userId: userId });
}

var fileTypes = [
	'image/jpeg',
	'image/pjpeg',
	'image/png'
]

function validFileType(file) {
	for (var i = 0; i < fileTypes.length; i++) {
		if (file.type === fileTypes[i]) {
			return true;
		}
	}

	return false;
}

function returnFileSize(number) {
	if (number < 1024) {
		return number + 'bytes';
	} else if (number > 1024 && number < 1048576) {
		return (number / 1024).toFixed(1) + 'KB';
	} else if (number > 1048576) {
		return (number / 1048576).toFixed(1) + 'MB';
	}
}

function updateImageDisplay() {

	var preview = document.getElementById("preview");
	var input = document.getElementById("input-image");
	while (preview.firstChild) {
		preview.removeChild(preview.firstChild);
	}

	var curFiles = input.files;
	if (curFiles.length === 0) {
		var para = document.createElement('p');
		para.textContent = 'No files currently selected for upload';
		preview.appendChild(para);
	} else {
		var list = document.createElement('ol');
		preview.appendChild(list);
		for (var i = 0; i < curFiles.length; i++) {
			var listItem = document.createElement('li');
			var para = document.createElement('p');
			if (validFileType(curFiles[i])) {
				para.textContent = 'File name ' + curFiles[i].name + ', file size ' + returnFileSize(curFiles[i].size) + '.';
				var image = document.createElement('img');
				image.src = window.URL.createObjectURL(curFiles[i]);

				listItem.appendChild(image);
				listItem.appendChild(para);

			} else {
				para.textContent = 'File name ' + curFiles[i].name + ': Not a valid file type. Update your selection.';
				listItem.appendChild(para);
			}

			list.appendChild(listItem);
		}
	}
}
