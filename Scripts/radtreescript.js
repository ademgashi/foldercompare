(function () {
    
    window.onClientContextMenuShowing = function (sender, args) {
        var treeNode = args.get_node();
        //treeNode.set_selected(true);
        //enable/disable menu items
        setMenuItemsState(args.get_menu().get_items(), treeNode);
    };

    window.onClientContextMenuShowingStaging = function (sender, args) {
        var treeNode = args.get_node();
        //treeNode.set_selected(true);
        //enable/disable menu items
        setMenuItemsStateStaging(args.get_menu().get_items(), treeNode);
    };

    window.onClientContextMenuItemClicking = function (sender, args) {
        var menuItem = args.get_menuItem();
        var treeNode = args.get_node();
        menuItem.get_menu().hide();
        var result;
        switch (menuItem.get_value()) {
            case "Copy":
                result = confirm("Are you sure you want to copy the item: " + treeNode.get_text());
                args.set_cancel(!result);
                break;
            case "ViewDiff":
                result = confirm("Are you sure you want to view differences for item: " + treeNode.get_text());
                args.set_cancel(!result);
                break;
            case "Delete":
                result = confirm("Are you sure you want to delete the item: " + treeNode.get_text());
                args.set_cancel(!result);
                break;
        }
    };

    function setMenuItemsStateStaging(menuItems, treeNode) {

        var isdifferent = (treeNode._element.className.indexOf('itisdifferent') >= 0);

        var isEmpty = treeNode._element.className.indexOf('exists_on_test') >= 0;
        
        
        for (var i = 0; i < menuItems.get_count() ; i++) {
            var menuItem = menuItems.getItem(i);
            // alert(menuItem.get_value());
            switch (menuItem.get_value()) {
                case "Copy":
                    if (isEmpty) {
                        // menuItem.set_text(String.format("Copy from stagingpath to testpath", treeNode.get_text()));
                        menuItem.set_text("Copy from test to staging");

                    } else {
                        menuItem.set_text("Copy from staging to test");

                    }
                    break;
                case "Delete":
                    if (isEmpty) {
                        menuItem.set_enabled(false);

                    } else {
                        menuItem.set_enabled(true);

                    }
                    break;
                case "ViewDiff":
                    if (isdifferent) {
                        menuItem.set_enabled(true);

                    } else {
                        menuItem.set_enabled(false);

                    }

                    break;

            }
        }
    }

    function setMenuItemsState(menuItems, treeNode) {
        
        var isdifferent = (treeNode._element.className.indexOf('itisdifferent') >= 0);

        //var isEmpty = treeNode.get_text() == '';
        var isEmpty = treeNode._element.className.indexOf('exists_on_staging') >= 0;
       
        for (var i = 0; i < menuItems.get_count() ; i++) {
            var menuItem = menuItems.getItem(i);
           // alert(menuItem.get_value());
            switch (menuItem.get_value()) {
                case "Copy":
                    if (isEmpty) {
                        // menuItem.set_text(String.format("Copy from stagingpath to testpath", treeNode.get_text()));
                        menuItem.set_text("Copy from staging to test");
                    } else {
                        menuItem.set_text("Copy from test to staging");
                    }
                    break;
                case "Delete":
                    if (isEmpty) {
                        menuItem.set_enabled(false);

                    } else {
                        menuItem.set_enabled(true);

                    }
                    break;
                case "ViewDiff":
                    if (isdifferent) {
                        menuItem.set_enabled(true);

                    } else {
                        menuItem.set_enabled(false);

                    }
              
                    break;
             
            }
        }
    }

    //formats the Text of the menu item
    function formatMenuItem(menuItem, treeNode, formatString) {
        var nodeValue = treeNode.get_value();
        if (nodeValue && nodeValue.indexOf("_Private_") == 0) {
            menuItem.set_enabled(false);
        }
        else {
            menuItem.set_enabled(true);
        }
        var newText = String.format(formatString, extractTitleWithoutMails(treeNode));
        menuItem.set_text(newText);
    }

 
}());