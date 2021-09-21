document.getElementById('table').addEventListener("scroll", function () {
    var translate = "translate(0," + this.scrollTop + "px)";
    var myElements = this.querySelectorAll("thead");
    //myElements.style.border = '2px solid red';
    for (var i = 0; i < myElements.length; i++) {
        myElements[i].style.transform = translate;
    }
});