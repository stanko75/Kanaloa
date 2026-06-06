<?php

$config = require dirname(__DIR__, 2) . '/kanaloaUploadConfig.php';

$USERNAME = $config['username'];
$PASSWORD = $config['password'];

$UPLOAD_DIR = __DIR__ . "/uploads/";
$MAX_FILE_SIZE = 20 * 1024 * 1024; // 20 MB

if (!isset($_SERVER['PHP_AUTH_USER'], $_SERVER['PHP_AUTH_PW']) ||
    $_SERVER['PHP_AUTH_USER'] !== $USERNAME ||
    $_SERVER['PHP_AUTH_PW'] !== $PASSWORD) {

    header('WWW-Authenticate: Basic realm="File Upload API"');
    http_response_code(401);
    echo json_encode(["success" => false, "message" => "Unauthorized"]);
    exit;
}

header("Content-Type: application/json");

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(["success" => false, "message" => "Only POST allowed"]);
    exit;
}

if (!isset($_FILES['file'])) {
    http_response_code(400);
    echo json_encode(["success" => false, "message" => "No file uploaded"]);
    exit;
}

// Folder parameter
$folder = $_POST['folder'] ?? '';
$folder = trim($folder, '/');

// Security: prevent path traversal
if (strpos($folder, '..') !== false) {
    http_response_code(400);
    echo json_encode(["success" => false, "message" => "Invalid folder"]);
    exit;
}

// Filename parameter
$fileName = $_POST['fileName'] ?? basename($_FILES['file']['name']);
$fileName = preg_replace('/[^a-zA-Z0-9._-]/', '_', $fileName);

$BASE_DIR = $_SERVER['DOCUMENT_ROOT'];
$targetDir = $BASE_DIR . '/' . $folder . '/';

if (!is_dir($targetDir)) {
    mkdir($targetDir, 0755, true);
}

$targetFile = $targetDir . $fileName;

$file = $_FILES['file'];

if ($file['error'] !== UPLOAD_ERR_OK) {
    http_response_code(400);
    echo json_encode(["success" => false, "message" => "Upload error"]);
    exit;
}

if ($file['size'] > $MAX_FILE_SIZE) {
    http_response_code(400);
    echo json_encode(["success" => false, "message" => "File too large"]);
    exit;
}

if (!move_uploaded_file($file['tmp_name'], $targetFile)) {
    http_response_code(500);
    echo json_encode(["success" => false, "message" => "Upload to " . $targetFile . " failed"]);
    exit;
}

echo json_encode([
    "success" => true,
    "savedTo" => $targetFile,
    "url" => "https://www.milosev.com/" . $folder . "/" . $fileName
]);