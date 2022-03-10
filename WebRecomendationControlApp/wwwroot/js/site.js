// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function () {
    // подписать кнопки пейджера на событие click
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