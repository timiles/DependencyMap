function openCorrespondingTabContent(selectedTabLink) {

    var tabLinks = document.getElementsByClassName('tab-link');
    var tabContents = document.getElementsByClassName('tab-content');
    for (var i = 0; i < tabLinks.length; i++) {

        if (tabLinks[i] === selectedTabLink) {
            tabLinks[i].className += ' active';
            tabContents[i].style.display = 'block';
        } else {
            // hide all tab contents
            tabLinks[i].className = tabLinks[i].className.replace(' active', '');
            tabContents[i].style.display = 'none';
        }
    }
}