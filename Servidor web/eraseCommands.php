<?php
//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
error_reporting(0);

function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',<>');
}
$com = filtrar($_POST['commands']);
$xml = simplexml_load_file('wee.xml');
foreach ($xml->version as $version)
{
	$numVersion = (int) $version['num'];
}
$numVersion++;
$xmlContent="<?xml version=\"1.0\"?>\r\n<instructions>\r\n<version num=\"" . $numVersion . "\" />\r\n";
$xmlContent = $xmlContent . "</instructions>";
//echo $xmlContent;
$fp = fopen('wee.xml',"w+");
fputs($fp, $xmlContent); 
fclose($fp);

Header("Location: configurar-ataque.php"); 
?>