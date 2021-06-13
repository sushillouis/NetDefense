<?php
	// Function which converts the level number to a string for csv display
	function level2string(int $level) {
		switch($level){
			case 0: return "Easy";
			case 1: return "Hard";
			case 2: return "Medium";
			default: return "Unknown";
		}
		return "Unknown";
	}

	// Error handling
	ini_set('display_errors', 1);
	error_reporting(E_ALL);

	// Establish a database connection
	$db = new SQLite3('/cse/home/jdahl/public_html/NetDefense-Backend/database.db');

	// Get the remote IP of the sender
	$remoteIP = isset($_SERVER['HTTP_CLIENT_IP']) ? $_SERVER['HTTP_CLIENT_IP'] : isset($_SERVER['HTTP_X_FORWARDED_FOR']) ? $_SERVER['HTTP_X_FORWARDED_FOR'] : $_SERVER['REMOTE_ADDR'];

	// Determine the database table to use
	$isWhiteHat = (boolean)(isset($_GET['hat']) ? $_GET['hat'] == "white" : true);
	$databaseHat = ($isWhiteHat ? "White" : "Black");

	if (isset($_GET['action']) && $_GET['action'] == "save"){ // Save user data
		// Get user data
		$name = $_GET['name'];
		$score = (int)$_GET['score'];
		$difficulty = $_GET['diff'];
		$startTime = $_GET['start'];
		$endTime = $_GET['end'];
		$updatesRemaining = $_GET['updates'];

		// Calculate UUID
		$uuid = md5($name . $remoteIP);

		// If the score currently in the database is greater than the new score... don't bother saving it
		$savedScore = $db->query("SELECT Score FROM Scores WHERE UUID='" . $uuid. "' AND Hat='" . $databaseHat . "' AND Difficulty='" . $difficulty . "';")->fetchArray();
		if($savedScore && $score < $savedScore[0]) return;

		// Add the row to the database (overwriting if it is already present)
		echo 'INSERT OR REPLACE INTO Scores (UUID, Hat, Difficulty, Name, Score, UpdatesRemaining, StartTime, EndTime) VALUES (\'' . $uuid . '\', \'' . $databaseHat . '\', \'' . $difficulty . '\', \'' . $name . '\', \'' . $score . '\', \'' . $updatesRemaining . '\', \'' . $startTime . '\', \'' . $endTime . '\'); ';
		$db->query('INSERT OR REPLACE INTO Scores (UUID, Hat, Difficulty, Name, Score, UpdatesRemaining, StartTime, EndTime) VALUES (\'' . $uuid . '\', \'' . $databaseHat . '\', \'' . $difficulty . '\', \'' . $name . '\', \'' . $score . '\', \'' . $updatesRemaining . '\', \'' . $startTime . '\', \'' . $endTime . '\'); ');
	} else if (isset($_GET['action']) && $_GET['action'] == "csv"){ // Export CSV file
		// Figure out difficulty
		$difficulty = (isset($_GET['diff']) ? "Difficulty='" .  $_GET['diff'] . "'" : "");
		// Determine the date format to use
		$dateFormat = (isset($_GET['fmt']) ? $_GET['fmt'] : "m/d/Y h:i:s A");

		// Determine the query that must be made to the table
		if(isset($_GET['hat']) && $_GET['hat'] == 'both') { $databaseHat = ""; $difficulty = (strlen($difficulty) > 0 ? "WHERE " . $difficulty : ""); }
		else { $databaseHat = "WHERE Hat='" . $databaseHat . "'" ; $difficulty = (strlen($difficulty) > 0 ? " AND " . $difficulty : ""); }
		$query = 'SELECT * FROM Scores ' . $databaseHat . " " . $difficulty . ';';

		$data = array(); // Variable storing the merged data which we output

		// Load the results into the array
		$results = $db->query($query);
		while($row = $results->fetchArray())
			$data[] = $row['UUID'] . ", " . $row['Hat'] .  ", " . level2string($row['Difficulty']) . ", " . $row['Name'] . ", " . $row['Score'] . ", " . $row['UpdatesRemaining'] . ", " . date($dateFormat, $row['StartTime']) . ", " . date($dateFormat, $row['EndTime']);

		// // If both hats are selected get the data from the other hat as well
		// if(isset($_GET['hat']) && $_GET['hat'] == "both"){
		// 	$results = $db->query('SELECT * FROM WhiteHatScores;');
		// 	while($row = $results->fetchArray())
		// 		$data[] = $row['UUID'] . ", WhiteHat, " . $row['Name'] . ", " . $row['Score'];
		// }

		// Make sure the data is sorted by UUID
		// sort($data);

		// Print out the data as standard CSV
		echo "UUID, HAT, DIFFICULTY, NAME, SCORE, UPDATES REMAINING, START TIME, END TIME\n";
		foreach ($data as &$row)
			echo $row . "\n";
	} else if (isset($_GET['action']) && $_GET['action'] == "purge"){ // Purge database file (TODO: this option should probably not be present outside of debugging)
		// Figure out difficulty
		$difficulty = (isset($_GET['diff']) ? "Difficulty='" .  $_GET['diff'] . "'" : "");

		// Determine the query that must be made to the table
		if(isset($_GET['hat']) && $_GET['hat'] == 'both') { $databaseHat = ""; $difficulty = (strlen($difficulty) > 0 ? "WHERE " . $difficulty : ""); }
		else { $databaseHat = "WHERE Hat='" . $databaseHat . "'" ; $difficulty = (strlen($difficulty) > 0 ? " AND " . $difficulty : ""); }
		$query = 'DELETE FROM Scores ' . $databaseHat . " " . $difficulty . ';';

		$results = $db->query($query);
		if($results) echo "database purged!";
	} else { // Fetch Leaderboard
		// Number of scores to fetch from the database
		$maxNumber = (isset($_GET['number']) ? $_GET['number'] : 5);
		// Figure out difficulty
		$difficulty = (isset($_GET['diff']) ? " AND Difficulty = \'" . $_GET['diff'] . "\'" : "");

		// Variable storing the data which will be converted to JSON
		$data = array();

		// Get the top <maxNumber> scores and prepare them for export
		$results = $db->query('SELECT * FROM Scores WHERE hat = \'' . $databaseHat . '\' ' . $difficulty . ' ORDER BY Score DESC LIMIT ' . $maxNumber . '; ');
		while($row = $results->fetchArray()){
			// Trim out all of the unnecessary data from the database
			$rowData = array();
			$rowData['uuid'] = $row['UUID'];
			$rowData['name'] = $row['Name'];
			$rowData['score'] = $row['Score'];
			// Add the row so that it will be converted to JSON
			$data[] = $rowData;
		}

		// Export the JSON data
		echo json_encode($data);
	}
?>
