Version: 1.0.0.0

Deployment:
(1) Close Notepad++
(2) Copy the dll into the Notepad++ plugins folder
(3) Start Notepad++, you should see a "DB Formatter" menu item in the "Plugins" menu and a "Format" toolbar button.
(4) Enter this text into an empty Notepad++ tab

	<Order:<Number:1><Date:01-08-2017><Customer:<Name:Company1><Address:Address1>><Items:<Item:<Name:Monitor><Quantity:2><Price:250>><Item:<Name:Harddisk><Quantity:3><Price:80>>>>

(5) Click the "Format" toolbar button or the "Plugins\DP Formatter\Format" menu
(6) The text should be formatted like this:

	<Order:
		<Number:1>
		<Date:01-08-2017>
		<Customer:
			<Name:Company1>
			<Address:Address1>
		>
		<Items:
			<Item:
				<Name:Monitor>
				<Quantity:2>
				<Price:250>
			>
			<Item:
				<Name:Harddisk>
				<Quantity:3>
				<Price:80>
			>
		>
	>
